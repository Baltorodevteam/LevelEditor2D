using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigator2D
{
	bool[,] collisionBoard;
	float[,] distanceBoard;
	GameObject parent;
	Vector2 size;

	public Navigator2D(int sizeX, int sizeY) 
	{
		size = new Vector2(sizeX, sizeY);

		collisionBoard = new bool[sizeX,sizeY];
		distanceBoard = new float[sizeX, sizeY];

		Bake();
	}

	public void Bake() 
	{
		Clear();
	}
	public void SetCollision(int x, int y, bool v)
    {
		if (x >= 0 && y >= 0 && x < size.x && y < size.y)
		{
			collisionBoard[x, y] = v;
		}
	}

	public void SetCollisionN(int x, int y, bool v = true)
    {
		SetCollision(x, y, v);
		SetCollision(x - 1, y, v);
		SetCollision(x + 1, y, v);
		SetCollision(x, y - 1, v);
		SetCollision(x, y + 1, v);
	}

	public List<Vector2> GetPath(Vector2 start, Vector2 end) 
	{
		ClearDistance();

		var toCheck = new List<Vector2>();
		toCheck.Add(start);
		SetDistance(start, 0.0f);

		while(toCheck.Count > 0) 
		{
			var current = toCheck[0];
			toCheck.RemoveAt(0);

			if (Vector2.SqrMagnitude(current - end) < 0.1f)
				break;

			float nextDistance = GetDistance(current) + 1.0f;
			float nextDistance2 = GetDistance(current) + 1.4142f;

			int currentX = (int)current.x;
			int currentY = (int)current.y;

			for (int x = currentX - 1; x <= currentX + 1; x++) 
			{
				if (x < 0 || x >= size.x)
					continue;

				for (int y = currentY - 1; y <= currentY + 1; y++) 
				{
					if (y < 0 || y >= size.y || (x == currentX && y == currentY))
						continue;

					var pos = new Vector2(x, y);
					var dis = nextDistance;

					if (x != currentX && y != currentY) 
					{
						if (!IsCollision(new Vector2(x, currentY)) && !IsCollision(new Vector2(currentX, y)))
							dis = nextDistance2;
						else
							continue;
					}

					if (!IsCollision(pos) && GetDistance(pos) > dis) 
					{
						SetDistance(pos, dis);
						toCheck.Add(pos);
					}
				}
			}
		}

		if (GetDistance(end) == float.MaxValue)
			return null;

		var path = new List<Vector2>();

		path.Add(end);
		Vector2 currnet = end;

		while (true) 
		{
			if (Vector2.SqrMagnitude(currnet - start) < 0.1f)
				break;

			var next = GetBestNeighbor(currnet);

			if (currnet == next)
				break;

			currnet = next;

			path.Insert(0, next);
		}

		return path;
	}

	private void SetDistance(Vector2 boardPosition, float distance) 
	{
		distanceBoard[(int)boardPosition.x, (int)boardPosition.y] = distance;
	}

	public bool IsCollision(Vector2 boardPosition) {
		return collisionBoard[(int)boardPosition.x, (int)boardPosition.y];
	}

	private float GetDistance(Vector2 boardPosition) 
	{
		return distanceBoard[(int)boardPosition.x, (int)boardPosition.y];
	}

	private Vector2 GetBestNeighbor(Vector2 position) 
	{
		float distance = float.MaxValue;
		Vector2 neighbor = position;

		int currentX = (int)position.x;
		int currentY = (int)position.y;

		for (int x = currentX - 1; x <= currentX + 1; x++) 
		{
			if (x < 0 || x >= size.x)
				continue;

			for (int y = currentY - 1; y <= currentY + 1; y++) 
			{
				if (y < 0 || y >= size.y || (x == currentX && y == currentY))
					continue;

				if (x != currentX && y != currentY)
				{
					continue;
				}

				if (x != currentX && y != currentY && (IsCollision(new Vector2(x, currentY)) || IsCollision(new Vector2(currentX, y))))
					continue;

				var pos = new Vector2(x, y);
				var d = GetDistance(pos);

				if (d < distance) 
				{
					distance = d;
					neighbor = pos;
				}
			}
		}

		return neighbor;
	}

	void ClearDistance() 
	{
		for (int x = 0; x < (int)size.x; x++) 
		{
			for (int y = 0; y < (int)size.y; y++) 
			{
				distanceBoard[x, y] = float.MaxValue;
			}
		}
	}

	void Clear() 
	{
		for(int x = 0; x < (int)size.x; x++) 
		{
			for(int y = 0; y < (int)size.y; y++) 
			{
				collisionBoard[x, y] = false;
			}
		}

		ClearDistance();
	}
}
