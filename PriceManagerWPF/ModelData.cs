using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public ModelData modeldata { get; set; }
    }

    public class Material
    {

        public string Name { get; set; }

        //Base64 strings
        public string map { get; set; }
        public string normalMap { get; set; }
        public string roughnessMap { get; set; }
        public string displacementMap { get; set; }

        //Colors
        public float[] color { get; set; }

        public float[] tiling { get; set; }
        public float roughness { get; set; }
        public float metalness { get; set; }
        public float normalScale { get; set; }
        public float displacementScale { get; set; }


        //If true and no reflectionMap = Use scene generated
        public bool UseReflections { get; set; }
        public string ReflectionMap { get; set; }

        public Material()
        {
            tiling = new float[2] { 1, 1 };
            roughness = 1;
            metalness = 0;
            normalScale = 1;
            color = new float[4] {1, 1, 1, 1};

        }
    }

    public class PriceData
    {

        public double Price { get; set; }
        public double Discount { get; set; }
        public string[] Pricetype { get; set; }

        public PriceData()
        {
            Pricetype = new string[] {
                "Sqm",
                "M",
                "Unit"
            };
        }


    }

    public class SizeData
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int Depth { get; set; }
        public float[] Angles { get; set; }
        public int HeightOffset { get; set; }
        public int RoofLength { get; set; }

        public Dictionary<string, float[]> SnapPoints = new Dictionary<string, float[]>();

    }

    public class ModelData
    {

        public string Name { get; set; }
        public List<string> OldNames { get; set; }
        public string Type { get; set; }
        public SizeData SizeData { get; set; }
        public PriceData PriceData { get; set; }

        public int MaterialSlots { get; set; }
        public Material[] Materials { get; set; }

        public string ModelJsonData { get; set; }
        public string ModelFileName { get; set; }
        public ThreeJsonModel JsonModel { get; set; }
        public ThreeJsonMaterial JsonMaterial { get; set; }

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
