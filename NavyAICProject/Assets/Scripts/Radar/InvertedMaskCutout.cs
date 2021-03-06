using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

/// <summary>
/// From Codemonkey's video "Make a Cutout Mask in Unity! (Inverted Mask)"
/// https://www.youtube.com/watch?v=XJJl19N2KFM
/// </summary>
public class InvertedMaskCutout : Image
{
	//public override Material materialForRendering => base.materialForRendering;
	public override Material materialForRendering 
	{ 
		get
		{
			Material material = new Material(base.materialForRendering);
			material.SetInt("_StencilComp",(int)CompareFunction.NotEqual);
			return material;
		}	
	}
}
