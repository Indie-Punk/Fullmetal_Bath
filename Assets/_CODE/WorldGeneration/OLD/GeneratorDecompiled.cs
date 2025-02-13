using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// Token: 0x02000002 RID: 2
public class Generator : MonoBehaviour
{
	// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
	private void Start()
	{
		this.EmptyMap();
		this.ApplyMap();
	}

	// Token: 0x06000002 RID: 2 RVA: 0x00002060 File Offset: 0x00000260
	private void RandomMap()
	{
		for (int i = 0; i < 256; i++)
		{
			for (int j = 0; j < 256; j++)
			{
				this.map[i, j] = Random.Range(0f, 1f);
			}
		}
	}

	// Token: 0x06000003 RID: 3 RVA: 0x000020AC File Offset: 0x000002AC
	private void EmptyMap()
	{
		for (int i = 0; i < 256; i++)
		{
			for (int j = 0; j < 256; j++)
			{
				this.map[i, j] = 0f;
			}
		}
	}

	// Token: 0x06000004 RID: 4 RVA: 0x000020EC File Offset: 0x000002EC
	private void PerlinMap()
	{
		float num = Random.Range(0f, 5f);
		for (int i = 0; i < 256; i++)
		{
			for (int j = 0; j < 256; j++)
			{
				this.map[i, j] = Mathf.PerlinNoise(num + (float)i / 25.6f, num + (float)j / 25.6f);
			}
		}
	}

	// Token: 0x06000005 RID: 5 RVA: 0x00002150 File Offset: 0x00000350
	private void PerlinWorm()
	{
		float num = Random.Range(0f, 5f);
		int num2 = Random.Range(0, 256);
		List<Vector2Int> list = new List<Vector2Int>();
		for (int i = 0; i < 256; i++)
		{
			list.Add(this.Rotators[(int)(Mathf.PerlinNoise(num + (float)i / 25.6f, num + (float)num2 / 25.6f) * (float)this.Rotators.Length)]);
		}
		Vector2Int a = new Vector2Int(Random.Range(0, 256), Random.Range(0, 256));
		for (int j = 0; j < list.Count; j++)
		{
			a += list[j];
			if (a.x >= 256)
			{
				a.x = 0;
			}
			if (a.y >= 256)
			{
				a.y = 0;
			}
			if (a.x < 0)
			{
				a.x = 255;
			}
			if (a.y < 0)
			{
				a.y = 255;
			}
			this.map[a.x, a.y] = 5f;
		}
	}

	// Token: 0x06000006 RID: 6 RVA: 0x00002288 File Offset: 0x00000488
	private void Cheese()
	{
		float num = Random.Range(0f, 5f);
		for (int i = 0; i < 256; i++)
		{
			for (int j = 0; j < 256; j++)
			{
				if (Mathf.PerlinNoise(num + (float)i / 25.6f, num + (float)j / 25.6f) > 0.7f)
				{
					this.map[i, j] = 1f;
				}
				else
				{
					this.map[i, j] = 0f;
				}
			}
		}
	}

	// Token: 0x06000007 RID: 7 RVA: 0x0000230C File Offset: 0x0000050C
	private void Spagetti()
	{
		float num = Random.Range(0f, 5f);
		for (int i = 0; i < 256; i++)
		{
			for (int j = 0; j < 256; j++)
			{
				if (Mathf.PerlinNoise(num + (float)i / 25.6f, num + (float)j / 25.6f) > 0.45f && Mathf.PerlinNoise(num + (float)i / 25.6f, num + (float)j / 25.6f) < 0.55f)
				{
					this.map[i, j] = 1f;
				}
				else
				{
					this.map[i, j] = 0f;
				}
			}
		}
	}

	// Token: 0x06000008 RID: 8 RVA: 0x000023B4 File Offset: 0x000005B4
	private void AddNoise()
	{
		float num = Random.Range(0f, 5f);
		float[,] array = new float[256, 256];
		for (int i = 1; i < 255; i++)
		{
			for (int j = 1; j < 255; j++)
			{
				int num2 = (((double)this.map[i - 1, j] <= 0.5) ? 1 : 0) + (((double)this.map[i - 1, j + 1] <= 0.5) ? 1 : 0) + (((double)this.map[i - 1, j - 1] <= 0.5) ? 1 : 0) + (((double)this.map[i + 1, j] <= 0.5) ? 1 : 0) + (((double)this.map[i + 1, j + 1] <= 0.5) ? 1 : 0) + (((double)this.map[i + 1, j - 1] <= 0.5) ? 1 : 0) + (((double)this.map[i, j - 1] <= 0.5) ? 1 : 0) + (((double)this.map[i, j + 1] <= 0.5) ? 1 : 0);
				if (num2 != 0 && num2 != 8 && this.map[i, j] <= 0.5f)
				{
					array[i, j] = this.map[i, j] + 0.012f * Mathf.PerlinNoise(num + (float)i / 12.8f, num + (float)j / 12.8f);
				}
				else
				{
					array[i, j] = this.map[i, j];
				}
			}
		}
		this.map = array;
	}

	// Token: 0x06000009 RID: 9 RVA: 0x00002598 File Offset: 0x00000798
	private void ApplyMap()
	{
		this.texture = new Texture2D(256, 256);
		for (int i = 0; i < 256; i++)
		{
			for (int j = 0; j < 256; j++)
			{
				this.texture.SetPixel(i, j, (this.map[i, j] > 0.5f) ? this.air : this.stone);
			}
		}
		this.texture.filterMode = FilterMode.Point;
		this.texture.Apply();
	}

	// Token: 0x0600000A RID: 10 RVA: 0x00002620 File Offset: 0x00000820
	private void Interpolate()
	{
		float[,] array = new float[256, 256];
		for (int i = 1; i < 255; i++)
		{
			for (int j = 1; j < 255; j++)
			{
				array[i, j] = (this.map[i - 1, j] + this.map[i - 1, j - 1] + this.map[i, j - 1] + this.map[i + 1, j - 1] + this.map[i + 1, j] + this.map[i + 1, j + 1] + this.map[i, j + 1] + this.map[i - 1, j + 1] + this.map[i, j]) / 9f;
			}
		}
		this.map = array;
	}

	// Token: 0x0600000B RID: 11 RVA: 0x00002714 File Offset: 0x00000914
	private void DayNight()
	{
		float[,] array = new float[256, 256];
		for (int i = 1; i < 255; i++)
		{
			for (int j = 1; j < 255; j++)
			{
				int num = (((double)this.map[i - 1, j] <= 0.5) ? 1 : 0) + (((double)this.map[i - 1, j + 1] <= 0.5) ? 1 : 0) + (((double)this.map[i - 1, j - 1] <= 0.5) ? 1 : 0) + (((double)this.map[i + 1, j] <= 0.5) ? 1 : 0) + (((double)this.map[i + 1, j + 1] <= 0.5) ? 1 : 0) + (((double)this.map[i + 1, j - 1] <= 0.5) ? 1 : 0) + (((double)this.map[i, j - 1] <= 0.5) ? 1 : 0) + (((double)this.map[i, j + 1] <= 0.5) ? 1 : 0);
				if ((double)this.map[i, j] <= 0.5)
				{
					if (num == 3 || num == 4 || num == 6 || num == 7 || num == 8)
					{
						array[i, j] = 0f;
					}
					else
					{
						array[i, j] = 1f;
					}
				}
				else if (num == 3 || num == 6 || num == 7 || num == 8)
				{
					array[i, j] = 0f;
				}
				else
				{
					array[i, j] = 1f;
				}
			}
		}
		this.map = array;
	}

	// Token: 0x0600000C RID: 12 RVA: 0x000028E8 File Offset: 0x00000AE8
	private void RandomWalker()
	{
		Vector2Int a = new Vector2Int(Random.Range(0, 256), Random.Range(0, 256));
		for (int i = 0; i < 30; i++)
		{
			a += new Vector2Int(Random.Range(-1, 2), Random.Range(-1, 2)) * 2;
			if (a.x >= 256)
			{
				a.x = 0;
			}
			if (a.y >= 256)
			{
				a.y = 0;
			}
			if (a.x < 0)
			{
				a.x = 255;
			}
			if (a.y < 0)
			{
				a.y = 255;
			}
			this.map[a.x, a.y] = 5f;
		}
	}

	// Token: 0x0600000D RID: 13 RVA: 0x000029BC File Offset: 0x00000BBC
	private void RandomLine()
	{
		Vector2Int vector2Int = new Vector2Int(Random.Range(0, 256), Random.Range(0, 256));
		Vector2Int rhs = new Vector2Int(Random.Range(0, 256), Random.Range(0, 256));
		int num = 0;
		while (vector2Int != rhs && num < 100)
		{
			num++;
			vector2Int += new Vector2Int(Random.Range(-1, 2), Random.Range(-1, 2)) * 2;
			if (vector2Int.x >= 256)
			{
				vector2Int.x = 0;
			}
			if (vector2Int.y >= 256)
			{
				vector2Int.y = 0;
			}
			if (vector2Int.x < 0)
			{
				vector2Int.x = 255;
			}
			if (vector2Int.y < 0)
			{
				vector2Int.y = 255;
			}
			if (vector2Int.x < rhs.x)
			{
				int num2 = vector2Int.x;
				vector2Int.x = num2 + 1;
			}
			else
			{
				int num2 = vector2Int.x;
				vector2Int.x = num2 - 1;
			}
			if (vector2Int.y < rhs.y)
			{
				int num2 = vector2Int.y;
				vector2Int.y = num2 + 1;
			}
			else
			{
				int num2 = vector2Int.y;
				vector2Int.y = num2 - 1;
			}
			this.map[vector2Int.x, vector2Int.y] = 8f;
		}
	}

	// Token: 0x0600000E RID: 14 RVA: 0x00002B20 File Offset: 0x00000D20
	// private void MathGen()
	// {
	// 	Vector2Int vector2Int = new Vector2Int(128 + Random.Range(-5, 6), 128 + Random.Range(-5, 6));
	// 	Vector2Int[] array = new Vector2Int[5];
	// 	for (int i = 0; i < array.Length; i++)
	// 	{
	// 		array[i] = new Vector2Int(Random.Range(0, 256), Random.Range(0, 256));
	// 	}
	// 	for (int j = 0; j < 256; j++)
	// 	{
	// 		for (int k = 0; k < 256; k++)
	// 		{
	// 			if (Mathf.Pow((float)(j - vector2Int.x), 2f) + Mathf.Pow((float)(k - vector2Int.y), 2f) < 30f)
	// 			{
	// 				this.map[j, k] = 4f;
	// 			}
	// 			for (int l = 0; l < array.Length; l++)
	// 			{
	// 				if (Mathf.Pow((float)(j - array[l].x), 2f) + Mathf.Pow((float)(k - array[l].y), 2f) < 15f)
	// 				{
	// 					this.map[j, k] = 4f;
	// 				}
	// 			}
	// 		}
	// 	}
	// 	foreach (Vector2Int vector2Int2 in array)
	// 	{
	// 		Vector2Int rhs = vector2Int;
	// 		int num = 0;
	// 		while (vector2Int2 != rhs && num < 100)
	// 		{
	// 			num++;
	// 			vector2Int2 += new Vector2Int(Random.Range(-1, 2), Random.Range(-1, 2) * 2);
	// 			if (vector2Int2.x >= 256)
	// 			{
	// 				vector2Int2.x = 0;
	// 			}
	// 			if (vector2Int2.y >= 256)
	// 			{
	// 				vector2Int2.y = 0;
	// 			}
	// 			if (vector2Int2.x < 0)
	// 			{
	// 				vector2Int2.x = 255;
	// 			}
	// 			if (vector2Int2.y < 0)
	// 			{
	// 				vector2Int2.y = 255;
	// 			}
	// 			if (vector2Int2.x < rhs.x)
	// 			{
	// 				int num2 = vector2Int2.x;
	// 				vector2Int2.x = num2 + 1;
	// 			}
	// 			else
	// 			{
	// 				int num2 = vector2Int2.x;
	// 				vector2Int2.x = num2 - 1;
	// 			}
	// 			if (vector2Int2.y < rhs.y)
	// 			{
	// 				int num2 = vector2Int2.y;
	// 				vector2Int2.y = num2 + 1;
	// 			}
	// 			else
	// 			{
	// 				int num2 = vector2Int2.y;
	// 				vector2Int2.y = num2 - 1;
	// 			}
	// 			this.map[vector2Int2.x, vector2Int2.y] = 8f;
	// 		}
	// 	}
	// }

	// Token: 0x0600000F RID: 15 RVA: 0x00002DA8 File Offset: 0x00000FA8
	private void Distribution()
	{
		int num = 5;
		int num2 = 5;
		int num3 = 256 / num;
		int num4 = 256 / num2;
		Vector2Int[] array = new Vector2Int[num * num2];
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				array[i + j * num] = new Vector2Int(num3 * i + num3 / 2, num4 * j + num4 / 2) + new Vector2Int(Random.Range(-num3 + 1, num3 - 1), Random.Range(-num4 + 1, num4 - 1));
			}
		}
		for (int k = 0; k < 256; k++)
		{
			for (int l = 0; l < 256; l++)
			{
				for (int m = 0; m < array.Length; m++)
				{
					if (array[m].x == k && array[m].y == l)
					{
						this.map[k, l] = 50f;
					}
				}
			}
		}
	}

	// Token: 0x06000010 RID: 16 RVA: 0x00002EAC File Offset: 0x000010AC
	private void Cube()
	{
		int num = 5;
		int num2 = 5;
		int num3 = 256 / num;
		int num4 = 256 / num2;
		Vector3Int[] array = new Vector3Int[num * num2];
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				array[i + j * num] = new Vector3Int(num3 * i + num3 / 2, num4 * j + num4 / 2, num3 / 3) + new Vector3Int(Random.Range(-num3 / 2 + 1, num3 / 2 - 1), Random.Range(-num4 / 2 + 1, num4 / 2 - 1), (Random.Range(0, 5) == 0) ? 0 : Random.Range(-num4 / 7 + 1, num4 / 7 - 1));
			}
		}
		for (int k = 0; k < 256; k++)
		{
			for (int l = 0; l < 256; l++)
			{
				for (int m = 0; m < array.Length; m++)
				{
					if (array[m].x + array[m].z >= k && array[m].x - array[m].z <= k && array[m].y + array[m].z >= l && array[m].y - array[m].z <= l)
					{
						this.map[k, l] = 1f;
					}
				}
			}
		}
	}

	// Token: 0x06000011 RID: 17 RVA: 0x00003050 File Offset: 0x00001250
	private void Graph()
	{
		int num = 5;
		int num2 = 5;
		int num3 = 256 / num;
		int num4 = 256 / num2;
		Vector4[] array = new Vector4[num * num2];
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				array[i + j * num] = new Vector4((float)(num3 * i + num3 / 2), (float)(num4 * j + num4 / 2), (float)(num3 / 6), (float)(num4 / 6)) + new Vector4((float)Random.Range(-num3 / 2 + 1, num3 / 2 - 1), (float)Random.Range(-num4 / 2 + 1, num4 / 2 - 1), (float)((Random.Range(0, 5) == 0) ? 0 : Random.Range(-num3 / 7 + 1, num3 / 7 - 1)), (float)((Random.Range(0, 5) == 0) ? 0 : Random.Range(-num4 / 7 + 1, num4 / 7 - 1)));
			}
		}
		for (int k = 0; k < 256; k++)
		{
			for (int l = 0; l < 256; l++)
			{
				for (int m = 0; m < array.Length; m++)
				{
					if (array[m].x + array[m].z >= (float)k && array[m].x - array[m].z <= (float)k && array[m].y + array[m].w >= (float)l && array[m].y - array[m].w <= (float)l)
					{
						this.map[k, l] = 1f;
					}
				}
			}
		}
		for (int n = 0; n < array.Length; n++)
		{
			if (array[n].w != 0f)
			{
				int num5 = n;
				int num6 = 0;
				while ((num5 == n || array[n].w == 0f) && num6 < 20)
				{
					num6++;
					num5 = Random.Range(0, array.Length);
				}
				if (Random.Range(0, 2) == 0)
				{
					this.DrawLine(new Vector2(array[n].x, array[n].y), new Vector2(array[n].x, array[num5].y));
					this.DrawLine(new Vector2(array[n].x, array[num5].y), new Vector2(array[num5].x, array[num5].y));
				}
				else
				{
					this.DrawLine(new Vector2(array[n].x, array[n].y), new Vector2(array[num5].x, array[n].y));
					this.DrawLine(new Vector2(array[num5].x, array[n].y), new Vector2(array[num5].x, array[num5].y));
				}
			}
		}
	}

	// Token: 0x06000012 RID: 18 RVA: 0x000033C4 File Offset: 0x000015C4
	private void DrawLine(Vector2 p1, Vector2 p2)
	{
		Vector2 vector = p1;
		float num = 1f / Mathf.Sqrt(Mathf.Pow(p2.x - p1.x, 2f) + Mathf.Pow(p2.y - p1.y, 2f));
		float num2 = 0f;
		while ((int)vector.x != (int)p2.x || (int)vector.y != (int)p2.y)
		{
			vector = Vector2.Lerp(p1, p2, num2);
			num2 += num;
			this.map[(int)vector.x, (int)vector.y] = 3f;
		}
	}

	// Token: 0x06000013 RID: 19 RVA: 0x00003464 File Offset: 0x00001664
	private void OnGUI()
	{
		GUI.DrawTexture(new Rect((float)Screen.width * 0.5f - (float)Screen.height * 0.5f, 0f, (float)Screen.height, (float)Screen.height), this.texture);
		if (GUI.Button(new Rect((float)Screen.width * 0.5f - 565f, (float)(Screen.height - 25), 170f, 25f), "Заполнить случайно"))
		{
			this.RandomMap();
			this.ApplyMap();
		}
		if (GUI.Button(new Rect((float)Screen.width * 0.5f - 565f, (float)(Screen.height - 50), 170f, 25f), "Заполнить пустотой"))
		{
			this.EmptyMap();
			this.ApplyMap();
		}
		if (GUI.Button(new Rect((float)Screen.width * 0.5f - 395f, (float)(Screen.height - 25), 150f, 25f), "Шум Перлина"))
		{
			this.PerlinMap();
			this.ApplyMap();
		}
		if (GUI.Button(new Rect((float)Screen.width * 0.5f - 395f, (float)(Screen.height - 50), 150f, 25f), "Шум для границ"))
		{
			this.AddNoise();
			this.ApplyMap();
		}
		if (GUI.Button(new Rect((float)Screen.width * 0.5f - 395f, (float)(Screen.height - 75), 150f, 25f), "Червь Перлина"))
		{
			this.PerlinWorm();
			this.ApplyMap();
		}
		if (GUI.Button(new Rect((float)Screen.width * 0.5f - 245f, (float)(Screen.height - 25), 180f, 25f), "Интерполировать 1х"))
		{
			this.Interpolate();
			this.ApplyMap();
		}
		if (GUI.Button(new Rect((float)Screen.width * 0.5f - 245f, (float)(Screen.height - 50), 180f, 25f), "Интерполировать 5х"))
		{
			for (int i = 0; i < 5; i++)
			{
				this.Interpolate();
			}
			this.ApplyMap();
		}
		if (GUI.Button(new Rect((float)Screen.width * 0.5f - 245f, (float)(Screen.height - 75), 180f, 25f), "Интерполировать 25х"))
		{
			for (int j = 0; j < 25; j++)
			{
				this.Interpolate();
			}
			this.ApplyMap();
		}
		if (GUI.Button(new Rect((float)Screen.width * 0.5f - 65f, (float)(Screen.height - 25), 130f, 25f), "День/Ночь 1х"))
		{
			this.DayNight();
			this.ApplyMap();
		}
		if (GUI.Button(new Rect((float)Screen.width * 0.5f - 65f, (float)(Screen.height - 50), 130f, 25f), "День/Ночь 5х"))
		{
			for (int k = 0; k < 5; k++)
			{
				this.DayNight();
			}
			this.ApplyMap();
		}
		if (GUI.Button(new Rect((float)Screen.width * 0.5f - 65f, (float)(Screen.height - 75), 130f, 25f), "День/Ночь 25х"))
		{
			for (int l = 0; l < 25; l++)
			{
				this.DayNight();
			}
			this.ApplyMap();
		}
		if (GUI.Button(new Rect((float)Screen.width * 0.5f + 65f, (float)(Screen.height - 25), 140f, 25f), "Random Walker 1х"))
		{
			this.RandomWalker();
			this.ApplyMap();
		}
		if (GUI.Button(new Rect((float)Screen.width * 0.5f + 65f, (float)(Screen.height - 50), 140f, 25f), "Random Walker 5х"))
		{
			for (int m = 0; m < 5; m++)
			{
				this.RandomWalker();
			}
			this.ApplyMap();
		}
		if (GUI.Button(new Rect((float)Screen.width * 0.5f + 205f, (float)(Screen.height - 25), 140f, 25f), "Random Line 1х"))
		{
			this.RandomLine();
			this.ApplyMap();
		}
		if (GUI.Button(new Rect((float)Screen.width * 0.5f + 205f, (float)(Screen.height - 50), 140f, 25f), "Random Line 5х"))
		{
			for (int n = 0; n < 5; n++)
			{
				this.RandomLine();
			}
			this.ApplyMap();
		}
		if (GUI.Button(new Rect((float)Screen.width * 0.5f + 205f, (float)(Screen.height - 75), 140f, 25f), "Тригонометрия"))
		{
			// this.MathGen();
			this.ApplyMap();
		}
		if (GUI.Button(new Rect((float)Screen.width * 0.5f + 345f, (float)(Screen.height - 25), 140f, 25f), "Расп. Точки"))
		{
			this.Distribution();
			this.ApplyMap();
		}
		if (GUI.Button(new Rect((float)Screen.width * 0.5f + 345f, (float)(Screen.height - 50), 140f, 25f), "Расп. Квадраты"))
		{
			this.Cube();
			this.ApplyMap();
		}
		if (GUI.Button(new Rect((float)Screen.width * 0.5f + 345f, (float)(Screen.height - 75), 140f, 25f), "Графы"))
		{
			this.Graph();
			this.ApplyMap();
		}
		if (GUI.Button(new Rect((float)Screen.width * 0.5f + 485f, (float)(Screen.height - 25), 140f, 25f), "Спагетти"))
		{
			this.Spagetti();
			this.ApplyMap();
		}
		if (GUI.Button(new Rect((float)Screen.width * 0.5f + 485f, (float)(Screen.height - 50), 140f, 25f), "Сыр"))
		{
			this.Cheese();
			this.ApplyMap();
		}
	}

	// Token: 0x04000001 RID: 1
	public Color air;

	// Token: 0x04000002 RID: 2
	public Color stone;

	// Token: 0x04000003 RID: 3
	private Texture2D texture;

	// Token: 0x04000004 RID: 4
	private const int WIDTH = 256;

	// Token: 0x04000005 RID: 5
	private const int HEIGHT = 256;

	// Token: 0x04000006 RID: 6
	private float[,] map = new float[256, 256];

	// Token: 0x04000007 RID: 7
	private Vector2Int[] Rotators = new Vector2Int[]
	{
		new Vector2Int(0, 0),
		new Vector2Int(-1, 0),
		new Vector2Int(-1, 1),
		new Vector2Int(0, 1),
		new Vector2Int(1, 1),
		new Vector2Int(1, 0),
		new Vector2Int(1, -1),
		new Vector2Int(0, -1),
		new Vector2Int(-1, -1)
	};
}
