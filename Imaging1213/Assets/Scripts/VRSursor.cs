using UnityEngine;
using System.Collections;

public class VRSursor : MonoBehaviour {

    public Sprite sprite;
	// Use this for initialization

	
	// Update is called once per frame
	void Update () {
	
	}

    void OnEnable()
    {

        if (sprite)
        {
            Texture2D src = sprite.texture;
            Rect rect = sprite.textureRect;
            Texture2D dst = new Texture2D((int)rect.width, (int)rect.height);
            Color[] pixels = src.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
            dst.SetPixels(pixels);

            Vector2 hotspot = 0.5f * new Vector2(rect.width, rect.height);
            Cursor.SetCursor(dst, hotspot, CursorMode.Auto);
            dst.Apply();

        }
    }

    void OnDisable()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
