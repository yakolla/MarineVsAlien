using UnityEngine;
using System.Collections;

public class Cooldown {



	static public Texture2D ProgressUpdate(Texture2D tex, float progress, Color overlayColor){
		progress = 1-progress;
		Texture2D thisTex = new Texture2D(tex.width, tex.height);
		Vector2 centre = new Vector2(Mathf.Ceil(thisTex.width/2), Mathf.Ceil(thisTex.height/2)); //find the centre pixel
		for(int y = 0; y < thisTex.height; y++){
			for(int x = 0; x < thisTex.width; x++){
				float angle = Mathf.Atan2(x-centre.x, y-centre.y)*Mathf.Rad2Deg*-1; //find the angle between the centre and this pixel (between -180 and 180)
				if(angle < 0){
					angle += 360; //change angles to go from 0 to 360
				}
				Color pixColor = tex.GetPixel(x, y);
				if(angle <= progress*360.0){ //if the angle is less than the progress angle blend the overlay colour
					pixColor = new Color(
						(pixColor.r*pixColor.a*(1-overlayColor.a))+(overlayColor.r*overlayColor.a),
						(pixColor.g*pixColor.a*(1-overlayColor.a))+(overlayColor.g*overlayColor.a),            
						(pixColor.b*pixColor.a*(1-overlayColor.a))+(overlayColor.b*overlayColor.a)            
						);
					thisTex.SetPixel(x, y, pixColor);
				}else{
					thisTex.SetPixel(x, y, pixColor);
				}
			}
		}
		thisTex.Apply(); //apply the cahnges we made to the texture
		return thisTex;
	}


}
