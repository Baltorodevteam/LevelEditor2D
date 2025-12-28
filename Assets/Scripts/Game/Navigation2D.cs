using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigation2D
{
	bool[][] collisionBoard;
	float[][] distanceBoard;
	GameObject parent;
	Vector2 size;

	public Navigation2D(GameObject parent, int sizeX, int sizeY) {
		this.parent = parent;
		size = new Vector2(sizeX, sizeY);

		collisionBoard = new bool[sizeX][];
		distanceBoard = new float[sizeX][];

		for (int i=0; i<sizeX; i++) {
			collisionBoard[i] = new bool[sizeY];
			distanceBoard[i] = new float[sizeY];
		}

		Bake();
	}

	public void Bake() {
		Clear();

		var colliders = parent.GetComponentsInChildren<Collider2D>(true);

		foreach (var collider in colliders) {
			//if (collider.tag == "Enemy" || collider.tag == "Player")
			//	continue;
			if (collider.tag != "Terrain" && collider.tag == "UnderWater_Terrain")
				continue;

			var position = collider.transform.position - parent.transform.position;

			collisionBoard[(int)Mathf.Floor(position.x + collider.offset.x)][(int)Mathf.Floor(position.y + collider.offset.y)] = true;
		}
	}

	public List<Vector3> GetPath(Vector3 from, Vector3 to) {
		ClearDistance();

		Vector2 start = GetBoardPosition(from);
		Vector2 end = GetBoardPosition(to);

		var toCheck = new List<Vector2>();
		toCheck.Add(start);
		SetDistance(start, 0.0f);

		while(toCheck.Count > 0) {
			var current = toCheck[0];
			toCheck.RemoveAt(0);

			if (Vector2.SqrMagnitude(current - end) < 0.1f)
				break;

			float nextDistance = GetDistance(current) + 1.0f;
			float nextDistance2 = GetDistance(current) + 1.4142f;

			int currentX = (int)current.x;
			int currentY = (int)current.y;

			for (int x = currentX - 1; x <= currentX + 1; x++) {
				if (x < 0 || x >= size.x)
					continue;

				for (int y = currentY - 1; y <= currentY + 1; y++) {
					if (y < 0 || y >= size.y || (x == currentX && y == currentY))
						continue;

					var pos = new Vector2(x, y);
					var dis = nextDistance;

					if (x != currentX && y != currentY) {
						if (!IsCollision(new Vector2(x, currentY)) && !IsCollision(new Vector2(currentX, y)))
							dis = nextDistance2;
						else
							continue;
					}

					if (!IsCollision(pos) && GetDistance(pos) > dis) {
						SetDistance(pos, dis);
						toCheck.Add(pos);
					}
				}
			}
		}

		if (GetDistance(end) == float.MaxValue)
			return null;

		var path = new List<Vector3>();

		path.Add(GetWorldPosition(end));
		Vector2 currnet = end;

		while (true) {
			if (Vector2.SqrMagnitude(currnet - start) < 0.1f)
				break;

			var next = GetBestNeighbor(currnet);

			if (currnet == next)
				break;

			currnet = next;

			path.Insert(0, GetWorldPosition(next));
		}

		return path;
	}

	private void SetDistance(Vector2 boardPosition, float distance) {
		distanceBoard[(int)boardPosition.x][(int)boardPosition.y] = distance;
	}

	public bool IsCollision(Vector2 boardPosition) {
		return collisionBoard[(int)boardPosition.x][(int)boardPosition.y];
	}

	private float GetDistance(Vector2 boardPosition) {
		return distanceBoard[(int)boardPosition.x][(int)boardPosition.y];
	}

	private Vector2 GetBestNeighbor(Vector2 position) {
		float distance = float.MaxValue;
		Vector2 neighbor = position;

		int currentX = (int)position.x;
		int currentY = (int)position.y;

		for (int x = currentX - 1; x <= currentX + 1; x++) {
			if (x < 0 || x >= size.x)
				continue;

			for (int y = currentY - 1; y <= currentY + 1; y++) {
				if (y < 0 || y >= size.y || (x == currentX && y == currentY))
					continue;

				if (x != currentX && y != currentY && (IsCollision(new Vector2(x, currentY)) || IsCollision(new Vector2(currentX, y))))
					continue;

				var pos = new Vector2(x, y);
				var d = GetDistance(pos);

				if (d < distance) {
					distance = d;
					neighbor = pos;
				}
			}
		}

		return neighbor;
	}

	public Vector3 GetWorldPosition(Vector2 position) {
		return parent.transform.position + new Vector3(position.x, position.y, 0.0f);
	}

	public Vector2 GetBoardPosition(Vector3 position) {
		var pos = position - parent.transform.position;
		pos.x = Mathf.Clamp(Mathf.Round(pos.x), 0, size.x - 1);
		pos.y = Mathf.Clamp(Mathf.Round(pos.y), 0, size.y - 1);

		return new Vector2(pos.x, pos.y);
	}

	void ClearDistance() {
		for (int x = 0; x < distanceBoard.Length; x++) {
			for (int y = 0; y < distanceBoard[x].Length; y++) {
				distanceBoard[x][y] = float.MaxValue;
			}
		}
	}

	void Clear() {
		for(int x=0; x<collisionBoard.Length; x++) {
			for(int y=0; y<collisionBoard[x].Length; y++) {
				collisionBoard[x][y] = false;
			}
		}

		ClearDistance();
	}

	public float[][] GetDistanceBoard() {
		return distanceBoard;
	}
}
