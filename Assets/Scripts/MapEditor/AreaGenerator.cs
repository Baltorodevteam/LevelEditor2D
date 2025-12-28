using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class AreaGenerator
{
	public static int MIRROR_NONE = 0;
	public static int MIRROR_X = 1;
	public static int MIRROR_Y = 2;
	public static int MIRROR_XY = 3;

	public static int TERRAIN_IDX = 1;
	public static int MARGIN_IDX = -1;

	public static int width;
	public static int height;

	int randomStartPercentValue = 20;
	public static int tresholdIfEmpty;
	public static int tresholdIfTaken;
	public static int maxIterations = 5;

	class LayerArea
    {
		public int[,] grid;

		public LayerArea(int w, int h)
        {
			grid = new int[w, h];
		}
	}

	int[,] areaGrid;
	LayerArea[] layerAreaGrid;

	AreaGeneratorParams areaGeneratorParams = new AreaGeneratorParams();


	public AreaGenerator()
    {
		AreaGeneratorParams.Load("area_generator_params.json", ref areaGeneratorParams);
	}


	public void GenerateDefault()
	{
		areaGrid = new int[width, height];

		RandomFillArea(false);
		for (int i = 0; i < maxIterations; i++)
		{
			Make();
			if (CountPercentOfTakenArea() >= areaGeneratorParams.finalExpectedPercentValue)
			{
				break;
			}
		}

		MakeEdges();
	}

	public void Generate(LevelData levelData, RoomData roomData)
    {
		AreaGenerator.width = (int)roomData.GetRoomWidth();
		AreaGenerator.height = (int)roomData.GetRoomHeight();

		layerAreaGrid = new LayerArea[roomData.GetLayersCount()];
		for (int i = 0; i < layerAreaGrid.Length; i++)
        {
			layerAreaGrid[i] = new LayerArea(width, height);
		}

		// terrain...
		if (Random.Range(0,101) <= areaGeneratorParams.perlinNoisePropability)
        {
			AutoRunPerlinNoise(levelData, roomData);
		}
        else
        {
			AutoRun(levelData, roomData);
		}

		// environment...
		for(int i = 0; i < roomData.GetLayersCount(); i++)
        {
			GenerateEnvironment(i);
		}

		// enemies...
		GenerateEnemies(roomData.difficulty, roomData.isEndRoom);
	}

	public bool AutoRun(LevelData levelData, RoomData roomData)
	{
		bool regularArea = Random.Range(0, 100) <= areaGeneratorParams.regularAreaPropability;

		areaGrid = new int[width, height];

		randomStartPercentValue = areaGeneratorParams.randomStartPercentValue + Random.Range(0, 8);

		bool terrainOK = false;

		if (regularArea)
		{
			AreaGenerator.tresholdIfEmpty = 3;
			AreaGenerator.tresholdIfTaken = 3;
		}
        else
        {
			AreaGenerator.tresholdIfEmpty = 4;
			AreaGenerator.tresholdIfTaken = 5;
		}

		AreaGenerator.maxIterations = 5;

		for(; ; )
        {
			RandomFillArea(false);
			for (int i = 0; i < maxIterations; i++)
			{
				Make();
				if (CountPercentOfTakenArea() >= areaGeneratorParams.finalExpectedPercentValue)
				{
					break;
				}
			}
			
			if (regularArea)
			{
				int mirrorType = Random.Range(0, 100);
				if (mirrorType < 50)
				{
					GenerateMirrorXY();
				}
				else if (mirrorType < 85)
				{
					GenerateMirrorX();
				}
				else
				{
					GenerateMirrorY();
				}
			}

			MakeEdges();

			if (CheckOneArea())
			{
				bool bOK = true;

				if(!TryAddDoorsUp(levelData, roomData) || !TryAddDoorsBottom(levelData, roomData) || !TryAddDoorsLeft(levelData, roomData) || !TryAddDoorsRight(levelData, roomData))
				{
					bOK = false;
                }

				if(bOK)
                {
					terrainOK = true;
					break;
				}
			}
		}

		return terrainOK;
	}

	PerlinNoise perlinNoise = new PerlinNoise();

	public bool AutoRunPerlinNoise(LevelData levelData, RoomData roomData)
	{
		AreaGenerator.width = (int)roomData.GetRoomWidth();
		AreaGenerator.height = (int)roomData.GetRoomHeight();

		areaGrid = new int[width, height];

		int xOffset = roomData.GetRoomGridX() * (int)RoomData.defaultRoomWidth;//(int)roomData.GetRoomWidth();
		int yOffset = roomData.GetRoomGridY() * (int)RoomData.defaultRoomHeight;//(int)roomData.GetRoomHeight();

		float scale = 3.0f;

		bool terrainOK = false;

		for(float j = 0; j < 100.0f; j++)
        {
			float minV = 0.2f;
			float maxV = 0.6f + j * 0.003f;

			perlinNoise.RandomOffsets();
			perlinNoise.Create(AreaGenerator.width, AreaGenerator.height, xOffset, yOffset, scale, minV, maxV);

			for(int w = 0; w < AreaGenerator.width; w++)
            {
				for (int h = 0; h < AreaGenerator.height; h++)
                {
					areaGrid[w, h] = perlinNoise.areaGrid[w, h] == 0 ? TERRAIN_IDX : 0;
				}
			}

			MakeEdges();

			ClearLonely();

			bool bOK = true;
			
			if (CheckOneArea())
			{
				if (!TryAddDoorsUp(levelData, roomData) || !TryAddDoorsBottom(levelData, roomData) || !TryAddDoorsLeft(levelData, roomData) || !TryAddDoorsRight(levelData, roomData))
				{
					bOK = false;
				}
				
				if(bOK)
				{
					terrainOK = true;
					break;
				}
			}
		}

		return terrainOK;
	}

	void GenerateEnvironment(int layer)
    {
		List<AreaObjectParams> obj = areaGeneratorParams.GetObjectsByLayer(layer);
		List<AreaObjectParams> everywhereObj = areaGeneratorParams.GetObjectsByMask(0, obj);
		List<AreaObjectParams> onGroundObj = areaGeneratorParams.GetObjectsByMask(1, obj);
		List<AreaObjectParams> offGroundObj = areaGeneratorParams.GetObjectsByMask(2, obj);

		ClearObjectsGrid(layer);

		GenerateEnvironmentFromList(layer, everywhereObj);
		GenerateEnvironmentFromList(layer, onGroundObj);
		GenerateEnvironmentFromList(layer, offGroundObj);
	}

	void GenerateEnvironmentFromList(int layer, List<AreaObjectParams> list)
    {
		if(list.Count == 0)
        {
			return;
        }

		int groundMask = list[0].groundMask;
		int groundIdx = -1;
		if(groundMask > 0)
        {
			groundIdx = groundMask == 1 ? groundIdx = TERRAIN_IDX : groundIdx = 0;
		}
		int propabilityPercent = areaGeneratorParams.enviroPropability[list[0].layerIndex];

		int size = 0;
		for (int k = 0; k < list.Count; k++)
		{
			size += (int)list[k].probabilityOfOccurrence;
		}
		int[] propTable = new int[size];
		int c = 0;
		for (int k = 0; k < list.Count; k++)
		{
			for (int j = 0; j < list[k].probabilityOfOccurrence; j++)
			{
				propTable[c] = list[k].objectID;
				c++;
			}
		}

		for (int row = 0; row < height; row++)
		{
			for (int column = 0; column < width; column++)
			{
				if(layerAreaGrid[layer].grid[column, row] != 0)
                {
					continue;
                }

				bool randomObject = false;

				if(groundMask == 0)
                {
					int r = Random.Range(0, 101);
					if (r > propabilityPercent)
                    {
						continue;
                    }
					randomObject = true;
				}
				else if (areaGrid[column, row] == groundIdx)
				{
					int numWalls = GetAdjacentPlaces(column, row, 1, 1);
					if(groundIdx == 0)
                    {
						if(numWalls > 0)
                        {
							continue;
                        }
					}
					int extPercent = 5 * (numWalls - 2);
					if (extPercent < 0)
					{
						extPercent = 0;
					}

					int r = Random.Range(0, 101);
					if (r > propabilityPercent + extPercent)
					{
						continue;
					}
					randomObject = true;
				}

				if(randomObject)
                {
					int rr = Random.Range(0, size);
					layerAreaGrid[layer].grid[column, row] = propTable[rr];
				}
			}
		}
	}

	bool CheckPlaceForObjectAt(int layer, int column, int row, int objectID)
    {
		AreaObjectParams aop = areaGeneratorParams.GetObjectByID(objectID);
		if (aop != null)
		{
			int left = column;
			int top = row;
			int right = left + aop.width;
			int bottom = row + aop.height;

			for (int x = left; x < right; x++)
			{
				for (int y = top; y < bottom; y++)
				{
					if (!IsOutOfBounds(x, y))
					{
						if (layerAreaGrid[layer].grid[x, y] != 0)
						{
							return false;
						}
					}
                    else
                    {
						return false;
                    }
				}
			}
			return true;
		}
		return false;
	}

	void SetObjectAt(int layer, int column, int row, int objectID)
    {
		AreaObjectParams aop = areaGeneratorParams.GetObjectByID(objectID);
		if(aop != null)
        {
			int left = column;
			int top = row;
			int right = left + aop.width;
			int bottom = row + aop.height;

			for(int x = left; x < right; x++)
            {
				for (int y = top; y < bottom; y++)
                {
					if(!IsOutOfBounds(x,y))
                    {
						layerAreaGrid[layer].grid[x, y] = MARGIN_IDX;
					}
				}
			}
			layerAreaGrid[layer].grid[column, row] = objectID;
		}
    }

	void GenerateEnemies(float difficulty, bool endRoom)
    {
		bool[] availEnemy = { true, true, false };

		if(endRoom)
        {
			availEnemy[0] = availEnemy[1] = false;
			availEnemy[2] = true;
		}

		EnemySpawnerSystem.Instance.PreparePlayerData(difficulty);
		EnemySpawnerSystem.Instance.PrepareEnemies();
		List<EnemyData> enemyList = EnemySpawnerSystem.Instance.GenerateWave(EnemySpawnerSystem.Instance.playerData, availEnemy);
		for(int i = 0; i < enemyList.Count; i++)
		{
			if(!FindGoodPlaceForEnemy(enemyList[i], true))
            {
				FindGoodPlaceForEnemy(enemyList[i], false);
			}
		}
	}

	bool FindGoodPlaceForEnemy(EnemyData ed, bool withTerrainMargin)
	{
		LayerArea layer = new LayerArea(width, height);

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				if (ed.layerMask == 1) // on ground
				{
					if(layer.grid[x, y] == 0)
                    {
						layer.grid[x, y] = 1;
					}
				}
				else if (ed.layerMask == 2) // off ground
				{
					if(areaGrid[x, y] > 0)
                    {
						layer.grid[x, y] = 1;
						if(withTerrainMargin)
                        {
							if (!IsOutOfBounds(x + 1, y)) layer.grid[x + 1, y] = 1;
							if (!IsOutOfBounds(x - 1, y)) layer.grid[x - 1, y] = 1;
							if (!IsOutOfBounds(x, y - 1)) layer.grid[x, y - 1] = 1;
							if (!IsOutOfBounds(x, y + 1)) layer.grid[x, y + 1] = 1;
							if (!IsOutOfBounds(x + 1, y + 1)) layer.grid[x + 1, y + 1] = 1;
							if (!IsOutOfBounds(x - 1, y - 1)) layer.grid[x - 1, y - 1] = 1;
							if (!IsOutOfBounds(x + 1, y - 1)) layer.grid[x + 1, y - 1] = 1;
							if (!IsOutOfBounds(x - 1, y + 1)) layer.grid[x - 1, y + 1] = 1;
						}
					}
				}
			}
		}

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				int id = layerAreaGrid[ed.layerIndex].grid[x, y];
				if (id > 0)
				{
					int width = 1;
					int height = 1;
					AreaObjectParams aop = areaGeneratorParams.GetObjectByID(id);
					if(aop != null)
                    {
						width = aop.width;
						height = aop.height;
					}

					for (int xx = x; xx < x + width; xx++)
					{
						for (int yy = y; yy < y + height; yy++)
						{
							if (!IsOutOfBounds(xx, yy))
							{
								layer.grid[xx, yy] = 1;
							}
						}
					}
				}
			}
		}

		List<int> freePlaces = new List<int>();
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				if(layer.grid[x, y] == 0)
                {
					freePlaces.Add(y * width + x);
                }
			}
		}

		if(freePlaces.Count > 0)
        {
			for (int c = 0; c < 100; c++)
			{
				int pos = freePlaces[Random.Range(0, freePlaces.Count)];
				int x = pos % width;
				int y = pos / width;
				if(CheckPlaceForObject(layer, x, y, ed))
                {
					layerAreaGrid[ed.layerIndex].grid[x, y] = ed.enemyID;
					return true;
				}
			}
		}
		return false;
	}

	bool CheckPlaceForObject(LayerArea layer, int x, int y, EnemyData ed)
    {
		for(int xx = x; xx < x + ed.areaGridW; xx++)
        {
			for (int yy = y; yy < y + ed.areaGridH; yy++)
            {
				if(layer.grid[xx, yy] > 0)
                {
					return false;
                }
            }
		}
		return true;
    }

	void GenerateMirrorX()
	{
		int w = width / 2;
		int h = height / 2;

		int[,] copyRegion1 = CopyRegion(0, 0, w, h);

		int[,] copyRegion2 = CopyRegion(0, height - h, w, h);

		ClearGrid();

		PasteRegion(copyRegion1, 0, 0,  MIRROR_NONE);
		PasteRegion(copyRegion1, width - w, 0, MIRROR_X);

		PasteRegion(copyRegion2, 0, height - h, MIRROR_NONE);
		PasteRegion(copyRegion2, width - w, height - h, MIRROR_X);
	}

	void GenerateMirrorY()
	{
		int w = width / 2;
		int h = height / 2;

		int[,] copyRegion1 = CopyRegion(0, 0, w, h);

		int[,] copyRegion2 = CopyRegion(width - w, 0, w, h);

		ClearGrid();

		PasteRegion(copyRegion1, 0, 0, MIRROR_NONE);
		PasteRegion(copyRegion1, 0, height - h, MIRROR_Y);

		PasteRegion(copyRegion2, width - w, 0, MIRROR_NONE);
		PasteRegion(copyRegion2, width - w, height - h, MIRROR_Y);
	}

	void GenerateMirrorXY()
	{
		int w = width / 2;
		int h = height / 2;

		int[,] copyRegion = CopyRegion(0, 0, w, h);

		ClearGrid();

		PasteRegion(copyRegion, 0, 0, MIRROR_NONE);
		PasteRegion(copyRegion, 0, height - h, MIRROR_Y);

		PasteRegion(copyRegion, width - w, 0, MIRROR_X);
		PasteRegion(copyRegion, width - w, height - h, MIRROR_XY);
	}


	public void RandomFillArea(bool withEdges = true)
	{
		// New, empty map
		areaGrid = new int[width, height];

		int mapMiddle = 0; // Temp variable
		for (int column = 0, row = 0; row < height; row++)
		{
			for (column = 0; column < width; column++)
			{
				if(withEdges)
                {
					// If coordinants lie on the the edge of the map (creates a border)
					if (column == 0)
					{
						areaGrid[column, row] = TERRAIN_IDX;
					}
					else if (row == 0)
					{
						areaGrid[column, row] = TERRAIN_IDX;
					}
					else if (column == width - 1)
					{
						areaGrid[column, row] = TERRAIN_IDX;
					}
					else if (row == height - 1)
					{
						areaGrid[column, row] = TERRAIN_IDX;
					}
				}
				else
                {
					mapMiddle = (height / 2);

					if (row == mapMiddle)
					{
						areaGrid[column, row] = 0;
					}
					else
					{
						areaGrid[column, row] = RandomPercent(randomStartPercentValue);
					}
				}
			}
		}
	}

	public void ClearGrid()
    {
        for (int row = 0; row < height; row++)
        {
            for (int column = 0; column < width; column++)
            {
                areaGrid[column, row] = 0;
            }
        }
    }

	public void ClearObjectsGrid(int layer)
	{
		for (int row = 0; row < height; row++)
		{
			for (int column = 0; column < width; column++)
			{
				layerAreaGrid[layer].grid[column, row] = 0;
			}
		}
	}

	public void MakeEdges()
	{
		for (int row = 0; row < height; row++)
		{
			areaGrid[0, row] = 1;
			areaGrid[width - 1, row] = TERRAIN_IDX;
		}
		for (int column = 0; column < width; column++)
		{
			areaGrid[column, 0] = 1;
			areaGrid[column, height - 1] = TERRAIN_IDX;
		}
	}


	public int CountPercentOfTakenArea()
	{
		int counter = 0;
		for (int row = 0; row < height; row++)
		{
			for (int column = 0; column < width; column++)
			{
				if(areaGrid[column, row] > 0)
                {
					counter++;
                }
			}
		}
		return (counter * 100) / (width * height);
	}

	public bool IsFree(int x, int y)
	{
		if (IsOutOfBounds(x, y))
		{
			return false;
		}

		if (areaGrid[x, y] > 0)
		{
			return false;
		}

		return true;
	}

	public int GetValue(int x, int y)
	{
		if (IsOutOfBounds(x, y))
		{
			return 0;
		}

		return areaGrid[x, y];
	}

	public int GetObjectValue(int layer, int x, int y)
	{
		if (IsOutOfBounds(x, y))
		{
			return 0;
		}

		return layerAreaGrid[layer].grid[x, y];
	}

	public void Make()
	{
		int[,] tmpAreaGrid = new int[width, height];

		for (int column = 0, row = 0; row < height; row++)
		{
			for (column = 0; column < width; column++)
			{
				tmpAreaGrid[column, row] = PlaceLogic(column, row, tresholdIfTaken, tresholdIfEmpty);
			}
		}

		areaGrid = tmpAreaGrid;
	}

	public void ClearLonely()
    {
		for (int row = 0; row < height; row++)
		{
			for (int column = 0; column < width; column++)
			{
				int numWalls = GetNeighbours(column, row);
				areaGrid[column, row] = (numWalls == 0 ? 0 : areaGrid[column, row]);
			}
		}
	}

	int GetNeighbours(int x, int y)
    {
		int counter = 0;
		if (x > 0 && areaGrid[x - 1, y] > 0) counter++;
		if (y > 0 && areaGrid[x, y - 1] > 0) counter++;
		if (x < width - 1 && areaGrid[x + 1, y] > 0) counter++;
		if (y < height - 1 && areaGrid[x, y + 1] > 0) counter++;
		return counter;
	}

	public int PlaceLogic(int x, int y, int tresholdIfTaken, int tresholdIfEmpty)
	{
		int numWalls = GetAdjacentPlaces(x, y, 1, 1);


		if (areaGrid[x, y] > 0)
		{
			if (numWalls >= tresholdIfTaken)
			{
				return TERRAIN_IDX;
			}
		}
		else
		{
			if (numWalls >= tresholdIfEmpty)
			{
				return TERRAIN_IDX;
			}
		}
		return 0;
	}

	public int GetAdjacentPlaces(int x, int y, int scopeX, int scopeY)
	{
		int startX = x - scopeX;
		int startY = y - scopeY;
		int endX = x + scopeX;
		int endY = y + scopeY;

		int iX = startX;
		int iY = startY;

		int wallCounter = 0;

		for (iY = startY; iY <= endY; iY++)
		{
			for (iX = startX; iX <= endX; iX++)
			{
				if (!(iX == x && iY == y))
				{
					if (!IsFree(iX, iY))
					{
						wallCounter += 1;
					}
				}
			}
		}
		return wallCounter;
	}

	bool IsOutOfBounds(int x, int y)
	{
		if (x < 0 || y < 0)
		{
			return true;
		}
		else if (x > width - 1 || y > height - 1)
		{
			return true;
		}
		return false;
	}

	int RandomPercent(int percent)
    {
        if (percent >= Random.Range(1, 101))
        {
            return TERRAIN_IDX;
        }
        return 0;
    }

	int[,] CopyRegion(int x, int y, int w, int h)
    {
		int[,] outRegion = new int[w, h];

		for (int ii = 0; ii < w; ii++)
		{
			for (int jj = 0; jj < h; jj++)
			{
				outRegion[ii, jj] = areaGrid[x + ii, y + jj];
			}
		}

		return outRegion;
    }

	void PasteRegion(int[,] srcRegion, int x, int y, int mirrorXY = 0)
    {
		int w = srcRegion.GetLength(0);
		int h = srcRegion.GetLength(1);

		if(mirrorXY == 0)
        {
			for (int ii = 0; ii < w; ii++)
			{
				for (int jj = 0; jj < h; jj++)
				{
					areaGrid[x + ii, y + jj] = srcRegion[ii, jj];
				}
			}
		}
		else if(mirrorXY == 1)
        {
			for (int ii = 0; ii < w; ii++)
			{
				for (int jj = 0; jj < h; jj++)
				{
					areaGrid[x + ii, y + jj] = srcRegion[w - 1 - ii, jj];
				}
			}
		}
		else if (mirrorXY == 2)
		{
			for (int ii = 0; ii < w; ii++)
			{
				for (int jj = 0; jj < h; jj++)
				{
					areaGrid[x + ii, y + jj] = srcRegion[ii, h - 1 - jj];
				}
			}
		}
		else if (mirrorXY == 3)
		{
			for (int ii = 0; ii < w; ii++)
			{
				for (int jj = 0; jj < h; jj++)
				{
					areaGrid[x + ii, y + jj] = srcRegion[w - 1 - ii, h - 1 - jj];
				}
			}
		}
	}

	bool CheckOneArea()
	{
		int[,] _areaGrid = new int[width, height];
		for (int w = 0; w < width; w++)
		{
			for (int h = 0; h < height; h++)
			{
				_areaGrid[w, h] = areaGrid[w, h] > 0 ? 1 : 0;
			}
		}

		int startX = -1;
		int startY = -1;
		for (int w = 0; w < width; w++)
		{
			for (int h = 0; h < height; h++)
			{
				if (_areaGrid[w, h] == 0)
				{
					startX = w;
					startY = h;
					break;
				}
			}
			if (startX >= 0)
			{
				break;
			}
		}

		int value1 = CountArae(0, _areaGrid);
		CheckDir4(startX, startY, _areaGrid);
		int value2 = CountArae(2, _areaGrid);

		return value1 == value2;
	}

	void CheckDir4(int x, int y, int[,] _areaGrid)
    {
		if(IsOutOfBounds(x, y))
        {
			return;
        }

		if(_areaGrid[x, y] != 0)
        {
			return;
        }

		_areaGrid[x, y] = 2;

		CheckDir4(x - 1, y, _areaGrid);
		CheckDir4(x + 1, y, _areaGrid);
		CheckDir4(x, y - 1, _areaGrid);
		CheckDir4(x, y + 1, _areaGrid);
	}

	int CountArae(int value, int[,] _areaGrid)
    {
		int counter = 0;
		for (int w = 0; w < width; w++)
		{
			for (int h = 0; h < height; h++)
			{
				if(_areaGrid[w, h] == value)
                {
					counter++;
                }
			}
		}
		return counter;
	}

	int CountNeighbour(int x, int y, int value)
    {
		int counter = 0;

		if (!IsOutOfBounds(x - 1, y) && areaGrid[x - 1, y] == value)
		{
			counter += 1;
		}
		if (!IsOutOfBounds(x + 1, y) && areaGrid[x + 1, y] == value)
		{
			counter += 1;
		}
		if (!IsOutOfBounds(x, y - 1) && areaGrid[x, y - 1] == value)
		{
			counter += 1;
		}
		if (!IsOutOfBounds(x, y + 1) && areaGrid[x, y + 1] == value)
		{
			counter += 1;
		}

		return counter;
	}

	bool TryAddDoorsBottom(LevelData ld, RoomData rd)
    {
		int levelGridX = rd.GetRoomGridX();
		int levelGridY = rd.GetRoomGridY();
		int levelGridW = (int)rd.GetRoomWidth() / (int)RoomData.defaultRoomWidth;
		int levelGridH = (int)rd.GetRoomHeight() / (int)RoomData.defaultRoomHeight;

		int startW = (int)RoomData.defaultRoomWidth / 2;
		int stopW = (levelGridW - 1) * (int)RoomData.defaultRoomWidth + startW;
		int h = 0;

		int gridX = 0;
		for(int w = startW; w <= stopW; w += (int)RoomData.defaultRoomWidth)
        {
			// there is heighbour at bottom
			if (ld.GetRoom(levelGridX + gridX, levelGridY - 1) != null)
            {

				bool bOK = false;
				for (int ii = 0; ii < 4; ii++)
				{
					if (CountNeighbour(w, h, 0) > 0)
					{
						bOK = true;
						break;
					}
					h++;
				}

				if (bOK)
				{
					for (int y = 0; y <= h; y++)
					{
						areaGrid[w, y] = 0;
					}
				}
				else
				{
					return false;
				}

			}

			gridX++;
		}

		return true;
	}

	bool TryAddDoorsUp(LevelData ld, RoomData rd)
	{
		int levelGridX = rd.GetRoomGridX();
		int levelGridY = rd.GetRoomGridY();
		int levelGridW = (int)rd.GetRoomWidth() / (int)RoomData.defaultRoomWidth;
		int levelGridH = (int)rd.GetRoomHeight() / (int)RoomData.defaultRoomHeight;

		int startW = (int)RoomData.defaultRoomWidth / 2;
		int stopW = (levelGridW - 1) * (int)RoomData.defaultRoomWidth + startW;
		int h = height - 1;

		int gridX = 0;
		for (int w = startW; w <= stopW; w += (int)RoomData.defaultRoomWidth)
		{
			// there is heighbour at bottom
			if (ld.GetRoom(levelGridX + gridX, levelGridY + levelGridH-1 + 1) != null)
			{

				bool bOK = false;
				for (int ii = 0; ii < 4; ii++)
				{
					if (CountNeighbour(w, h, 0) > 0)
					{
						bOK = true;
						break;
					}
					h--;
				}

				if (bOK)
				{
					for (int y = height - 1; y >= h; y--)
					{
						areaGrid[w, y] = 0;
					}
				}
                else
                {
					return false;
                }
			}

			gridX++;
		}

		return true;
	}

	bool TryAddDoorsLeft(LevelData ld, RoomData rd)
	{
		int levelGridX = rd.GetRoomGridX();
		int levelGridY = rd.GetRoomGridY();
		int levelGridW = (int)rd.GetRoomWidth() / (int)RoomData.defaultRoomWidth;
		int levelGridH = (int)rd.GetRoomHeight() / (int)RoomData.defaultRoomHeight;

		int startH = (int)RoomData.defaultRoomHeight / 2;
		int stopH = (levelGridH - 1) * (int)RoomData.defaultRoomHeight + startH;
		int w = 0;

		int gridY = 0;
		for (int h = startH; h <= stopH; h += (int)RoomData.defaultRoomHeight)
		{
			// there is heighbour at left
			if (ld.GetRoom(levelGridX - 1, levelGridY + gridY) != null)
			{

				bool bOK = false;
				for (int ii = 0; ii < 4; ii++)
				{
					if (CountNeighbour(w, h, 0) > 0)
					{
						bOK = true;
						break;
					}
					w++;
				}

				if (bOK)
				{
					for (int x = 0; x <= w; x++)
					{
						areaGrid[x, h] = 0;
					}
				}
                else
                {
					return false;
                }

			}

			gridY++;
		}

		return true;
	}

	bool TryAddDoorsRight(LevelData ld, RoomData rd)
	{
		int levelGridX = rd.GetRoomGridX();
		int levelGridY = rd.GetRoomGridY();
		int levelGridW = (int)rd.GetRoomWidth() / (int)RoomData.defaultRoomWidth;
		int levelGridH = (int)rd.GetRoomHeight() / (int)RoomData.defaultRoomHeight;

		int startH = (int)RoomData.defaultRoomHeight / 2;
		int stopH = (levelGridH - 1) * (int)RoomData.defaultRoomHeight + startH;
		int w = width - 1;

		int gridY = 0;
		for (int h = startH; h <= stopH; h += (int)RoomData.defaultRoomHeight)
		{
			// there is heighbour at right
			if (ld.GetRoom(levelGridX + levelGridW-1 + 1, levelGridY + gridY) != null)
			{

				bool bOK = false;
				for (int ii = 0; ii < 4; ii++)
				{
					if (CountNeighbour(w, h, 0) > 0)
					{
						bOK = true;
						break;
					}
					w--;
				}

				if (bOK)
				{
					for (int x = width - 1; x >= w; x--)
					{
						areaGrid[x, h] = 0;
					}
				}
                else
                {
					return false;
                }

			}

			gridY++;
		}

		return true;
	}


	//////////////////////////////////////////////////////////////////////////////////////////////////////////
	/////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public void Generate(LevelInfo levelData, RoomInfo roomData)
	{
		AreaGenerator.width = (int)roomData.w * (int)RoomData.defaultRoomWidth;
		AreaGenerator.height = (int)roomData.h * (int)RoomData.defaultRoomHeight;

		layerAreaGrid = new LayerArea[roomData.layers.Length];
		for (int i = 0; i < layerAreaGrid.Length; i++)
		{
			layerAreaGrid[i] = new LayerArea(width, height);
		}

		// terrain...
		if (Random.Range(0, 101) <= areaGeneratorParams.perlinNoisePropability)
		{
			AutoRunPerlinNoise(levelData, roomData);
		}
		else
		{
			AutoRun(levelData, roomData);
		}

		// environment...
		for(int i = 0; i < roomData.layers.Length; i++)
        {
			GenerateEnvironment(i);
		}

		// enemies...
		GenerateEnemies(roomData.difficulty, levelData.GetEndRoom() == roomData);

		CreateFromAreaGenerator(ref roomData);
	}

	public bool AutoRun(LevelInfo levelInfo, RoomInfo roomInfo)
	{
		bool regularArea = Random.Range(0, 100) <= areaGeneratorParams.regularAreaPropability;

		AreaGenerator.width = (int)roomInfo.w * (int)RoomData.defaultRoomWidth;
		AreaGenerator.height = (int)roomInfo.h * (int)RoomData.defaultRoomHeight;

		areaGrid = new int[width, height];

		randomStartPercentValue = areaGeneratorParams.randomStartPercentValue + Random.Range(0, 8);

		bool terrainOK = false;

		if (regularArea)
		{
			AreaGenerator.tresholdIfEmpty = 3;
			AreaGenerator.tresholdIfTaken = 3;
		}
		else
		{
			AreaGenerator.tresholdIfEmpty = 4;
			AreaGenerator.tresholdIfTaken = 5;
		}

		AreaGenerator.maxIterations = 5;

		for (; ; )
		{
			RandomFillArea(false);
			for (int i = 0; i < maxIterations; i++)
			{
				Make();
				if (CountPercentOfTakenArea() >= areaGeneratorParams.finalExpectedPercentValue)
				{
					break;
				}
			}

			if (regularArea)
			{
				int mirrorType = Random.Range(0, 100);
				if (mirrorType < 50)
				{
					GenerateMirrorXY();
				}
				else if (mirrorType < 85)
				{
					GenerateMirrorX();
				}
				else
				{
					GenerateMirrorY();
				}
			}

			MakeEdges();

			if (CheckOneArea())
			{
				bool bOK = true;

				if (!TryAddDoorsUp(levelInfo, roomInfo) || !TryAddDoorsBottom(levelInfo, roomInfo) || !TryAddDoorsLeft(levelInfo, roomInfo) || !TryAddDoorsRight(levelInfo, roomInfo))
				{
					bOK = false;
				}

				if (bOK)
				{
					terrainOK = true;
					break;
				}
			}
		}

		return terrainOK;
	}

	public bool AutoRunPerlinNoise(LevelInfo levelInfo, RoomInfo roomInfo)
	{
		AreaGenerator.width = (int)roomInfo.w * (int)RoomData.defaultRoomWidth;
		AreaGenerator.height = (int)roomInfo.h * (int)RoomData.defaultRoomHeight;

		areaGrid = new int[width, height];

		int xOffset = roomInfo.x * (int)RoomData.defaultRoomWidth/*(int)roomData.GetRoomWidth()*/;
		int yOffset = roomInfo.y * (int)RoomData.defaultRoomHeight/*(int)roomData.GetRoomHeight()*/;

		float scale = 3.0f;

		bool terrainOK = false;

		for (float j = 0; j < 100.0f; j++)
		{
			float minV = 0.2f;
			float maxV = 0.6f + j * 0.003f;

			perlinNoise.Create(AreaGenerator.width, AreaGenerator.height, xOffset, yOffset, scale, minV, maxV);

			for (int w = 0; w < AreaGenerator.width; w++)
			{
				for (int h = 0; h < AreaGenerator.height; h++)
				{
					areaGrid[w, h] = perlinNoise.areaGrid[w, h] == 0 ? TERRAIN_IDX : 0;
				}
			}

			MakeEdges();

			ClearLonely();

			bool bOK = true;

			if (CheckOneArea())
			{
				if (!TryAddDoorsUp(levelInfo, roomInfo) || !TryAddDoorsBottom(levelInfo, roomInfo) || !TryAddDoorsLeft(levelInfo, roomInfo) || !TryAddDoorsRight(levelInfo, roomInfo))
				{
					bOK = false;
				}

				if (bOK)
				{
					terrainOK = true;
					break;
				}
			}
		}

		return terrainOK;
	}

	bool TryAddDoorsBottom(LevelInfo ld, RoomInfo rd)
	{
		int levelGridX = rd.x;
		int levelGridY = rd.y;
		int levelGridW = rd.w;
		int levelGridH = rd.h;

		int startW = (int)RoomData.defaultRoomWidth / 2;
		int stopW = (levelGridW - 1) * (int)RoomData.defaultRoomWidth + startW;
		int h = 0;

		int gridX = 0;
		for (int w = startW; w <= stopW; w += (int)RoomData.defaultRoomWidth)
		{
			// // there is heighbour at top
			if (ld.GetRoom(levelGridX + gridX, levelGridY - 1) != null)
			{

				bool bOK = false;
				for (int ii = 0; ii < 4; ii++)
				{
					if (CountNeighbour(w, h, 0) > 0)
					{
						bOK = true;
						break;
					}
					h++;
				}

				if (bOK)
				{
					for (int y = 0; y <= h; y++)
					{
						areaGrid[w, y] = 0;
					}
				}
				else
				{
					return false;
				}

			}

			gridX++;
		}

		return true;
	}

	bool TryAddDoorsUp(LevelInfo ld, RoomInfo rd)
	{
		int levelGridX = rd.x;
		int levelGridY = rd.y;
		int levelGridW = rd.w;
		int levelGridH = rd.h;

		int startW = (int)RoomData.defaultRoomWidth / 2;
		int stopW = (levelGridW - 1) * (int)RoomData.defaultRoomWidth + startW;
		int h = height - 1;

		int gridX = 0;
		for (int w = startW; w <= stopW; w += (int)RoomData.defaultRoomWidth)
		{
			// there is heighbour at bottom
			if (ld.GetRoom(levelGridX + gridX, levelGridY + levelGridH - 1 + 1) != null)
			{

				bool bOK = false;
				for (int ii = 0; ii < 4; ii++)
				{
					if (CountNeighbour(w, h, 0) > 0)
					{
						bOK = true;
						break;
					}
					h--;
				}

				if (bOK)
				{
					for (int y = height - 1; y >= h; y--)
					{
						areaGrid[w, y] = 0;
					}
				}
				else
				{
					return false;
				}
			}

			gridX++;
		}

		return true;
	}

	bool TryAddDoorsLeft(LevelInfo ld, RoomInfo rd)
	{
		int levelGridX = rd.x;
		int levelGridY = rd.y;
		int levelGridW = rd.w;
		int levelGridH = rd.h;

		int startH = (int)RoomData.defaultRoomHeight / 2;
		int stopH = (levelGridH - 1) * (int)RoomData.defaultRoomHeight + startH;
		int w = 0;

		int gridY = 0;
		for (int h = startH; h <= stopH; h += (int)RoomData.defaultRoomHeight)
		{
			// there is heighbour at left
			if (ld.GetRoom(levelGridX - 1, levelGridY + gridY) != null)
			{

				bool bOK = false;
				for (int ii = 0; ii < 4; ii++)
				{
					if (CountNeighbour(w, h, 0) > 0)
					{
						bOK = true;
						break;
					}
					w++;
				}

				if (bOK)
				{
					for (int x = 0; x <= w; x++)
					{
						areaGrid[x, h] = 0;
					}
				}
				else
				{
					return false;
				}

			}

			gridY++;
		}

		return true;
	}

	bool TryAddDoorsRight(LevelInfo ld, RoomInfo rd)
	{
		int levelGridX = rd.x;
		int levelGridY = rd.y;
		int levelGridW = rd.w;
		int levelGridH = rd.h;

		int startH = (int)RoomData.defaultRoomHeight / 2;
		int stopH = (levelGridH - 1) * (int)RoomData.defaultRoomHeight + startH;
		int w = width - 1;

		int gridY = 0;
		for (int h = startH; h <= stopH; h += (int)RoomData.defaultRoomHeight)
		{
			// there is heighbour
			if (ld.GetRoom(levelGridX + levelGridW - 1 + 1, levelGridY + gridY) != null)
			{

				bool bOK = false;
				for (int ii = 0; ii < 4; ii++)
				{
					if (CountNeighbour(w, h, 0) > 0)
					{
						bOK = true;
						break;
					}
					w--;
				}

				if (bOK)
				{
					for (int x = width - 1; x >= w; x--)
					{
						areaGrid[x, h] = 0;
					}
				}
				else
				{
					return false;
				}

			}

			gridY++;
		}

		return true;
	}

	public bool CreateFromAreaGenerator(ref RoomInfo ri)
	{
		for (int l = 0; l < ri.layers.Length; l++)
		{
			ri.layers[l].Clear();

			for (int i = 0; i < ri.layers[0].w; i++)
			{
				for (int j = 0; j < ri.layers[0].h; j++)
				{
					int id = GetObjectValue(l, i, j);
					ri.layers[l].layerGrid[i, j] = id > 0 ? id : 0;
				}
			}
		}

		for (int l = 0; l < areaGeneratorParams.groundLayers.Length; l++)
        {
			ri.layers[areaGeneratorParams.groundLayers[l]].Clear();
			for (int i = 0; i < ri.layers[areaGeneratorParams.groundLayers[l]].w; i++)
			{
				for (int j = 0; j < ri.layers[areaGeneratorParams.groundLayers[l]].h; j++)
				{
					int gridV = IsFree(i, j) ? 0 : 1;
					if (gridV > 0) // terrain
					{
						ri.layers[areaGeneratorParams.groundLayers[l]].layerGrid[i, j] = 1;
					}
				}
			}
		}

		return true;
	}

}