﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PriceManagerWPF
{

    public class ThreeJsonMaterial
    {
        public bool depthWrite = false;
        public float[] colorEmissive = new float[3];
        public float opacity = 0;
        public string shading = "phong";
        public bool depthTest = true;
        public bool visible = true;
        public float[] specularColor = new float[3];
        public int blending = 1;
        public int specularCoef = 85;
        public float[] colorDiffuse = new float[3];
        public bool wireframe = false;
        public bool doubleSided = false;
        public bool transparent = false;
        public string DbgColor = "";
        public int DbgIndex = 0;
        public string DbgName = "";
    }

    public class MetaData
    {
        public int normals { get; set; }
        public int uvs { get; set; }
        public int faces { get; set; }
        public int vertices { get; set; }
        public string generator { get; set; }
        public string type { get; set; }
        public int materials { get; set; }
        public string version { get; set; }
    }

    public class ThreeJsonModel
    {
        public float[] normals { get; set; }
        public float[] vertices { get; set; }
        public List<float[]> uvs { get; set; }
        public int[] faces { get; set; }
        public ThreeJsonMaterial[] materials { get; set; }
        public MetaData metadata { get; set; }

    }

    public class Material
    {

        public string Name { get; set; }

        //Base64 strings
        public string Map { get; set; }
        public string NormalMap { get; set; }
        public string RoughnessMap { get; set; }
        public string DisplacementMap { get; set; }

        public float[] Tiling { get; set; }
        public float Roughness { get; set; }
        public float Metalness { get; set; }
        public float NormalScale { get; set; }
        public float DisplacementScale { get; set; }


        //If true and no reflectionMap = Use scene generated
        public bool UseReflections { get; set; }
        public string ReflectionMap { get; set; }


    }

    public class PriceData
    {
        public enum PriceType
        {
            Sqm,
            M,
            Unit
        }

        public double Price { get; set; }
        public double Discount { get; set; }
        public string Currency = "kr";
        public PriceType Pricetype = PriceType.Unit;

    }

    public class SizeData
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int Depth { get; set; }
        public float AngleDeg { get; set; }

        public Dictionary<string, float[]> SnapPoints = new Dictionary<string, float[]>();


    }

    public class ModelData
    {

        public string Name { get; set; }
        public string Type { get; set; }
        public SizeData SizeData = new SizeData();
        public PriceData PriceData = new PriceData();

        public int MaterialSlots { get; set; }
        public Material[] Materials { get; set; }

        public string ModelJsonData { get; set; }
        public string ModelFileName { get; set; }

        public ModelData()
        {
            Name = "";
            Type = "";
            SizeData = new SizeData();
            PriceData = new PriceData();
        }

        public ModelData(string name, string type)
        {

        }

        public void AddSnapPoint(string name, float[] vector)
        {
            SizeData.SnapPoints.Add(name, vector);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}