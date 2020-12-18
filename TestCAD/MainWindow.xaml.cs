﻿using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.Wpf.SharpDX;
using SharpDX.Direct3D11;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;

namespace TestCAD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private EffectsManager manager;
        private Viewport3DX viewport;
        private SceneNodeGroupModel3D sceneNodeGroup;
        private PhongMaterialCollection materials = new PhongMaterialCollection();
        private Random rnd = new Random();
        private System.Windows.Media.Media3D.Transform3DGroup group = new System.Windows.Media.Media3D.Transform3DGroup();

        public MainWindow()
        {
            
            InitializeComponent();

            manager = new DefaultEffectsManager();
            viewport = new Viewport3DX();
            viewport.BackgroundColor = Colors.White;
            viewport.ShowCoordinateSystem = true;
            viewport.EnableOITRendering = true;
            viewport.EffectsManager = manager;
            viewport.Items.Add(new DirectionalLight3D() { Direction = new System.Windows.Media.Media3D.Vector3D(-1, -1, -1) });
            viewport.Items.Add(new AmbientLight3D() { Color = Color.FromArgb(255, 50, 50, 50) });
            sceneNodeGroup = new SceneNodeGroupModel3D();
            viewport.Items.Add(sceneNodeGroup);
            viewport.Items.Add(new AxisPlaneGridModel3D());
            mainGrid.Children.Add(viewport);

            WindowState = WindowState.Maximized;
        }

        private void Button_Click_Add(object sender, RoutedEventArgs e) //добавление куба на сцену
        {
            
            BaseModel m = new Box_Model();
            viewport.Items.Add(VisualFigure(m));
            viewport.Items.Add(VisualEdges(m));
            viewport.Items.Add(VisualFace(m));

        }
        private void Button_Click_Remove(object sender, RoutedEventArgs e) //удаление последней фигуры со сцены  со сцены
        {
            viewport.Items.RemoveAt(viewport.Items.Count - 1);
        }


        private void Button_Click_AddSphere(object sender, RoutedEventArgs e) //добавление сферы на сцену
        {
            BaseModel m = new Sphere_Model();
            viewport.Items.Add(VisualFigure(m));
            viewport.Items.Add(VisualEdges(m));
            viewport.Items.Add(VisualFace(m));
        }
        private void Button_Click_AddCylinder(object sender, RoutedEventArgs e) //добавление цилиндра на сцену
        {
            BaseModel m = new Cylinder_Model();
            viewport.Items.Add(VisualFigure(m));
            viewport.Items.Add(VisualEdges(m));
            viewport.Items.Add(VisualFace(m));
        }
        private void Button_Click_AddGlass(object sender, RoutedEventArgs e) //добавление чаши на сцену
        {
            BaseModel m = new Revolved();
            viewport.Items.Add(VisualFigure(m));
            viewport.Items.Add(VisualEdges(m));
            viewport.Items.Add(VisualFace(m));
        }

        private void Button_Click_AddExtrusion(object sender, RoutedEventArgs e) //добавление выдавливания на сцену
        {
            BaseModel m = new Extrusion();
            viewport.Items.Add(VisualFigure(m));
            viewport.Items.Add(VisualEdges(m));
            viewport.Items.Add(VisualFace(m));
        }
        //private void buttonSceneNode_Click(object sender, RoutedEventArgs e) //вывод рандомной фигуры через SceneNode
        //{           

        //    var rmp = new UICompositeManipulator3D();
        //    rmp.Bind((GeometryModel3D)viewport.Items.Last());
        //    viewport.Items.Add(rmp);
            
        //}

        private void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            mainGrid.Children.Remove(viewport);
            
        }
        private LineGeometryModel3D VisualEdges(BaseModel m)
        {
            m.Update();
            var inxs2 = new IntCollection();
            m.Edges.ForEach(x2 => inxs2.AddAll(x2.Indices));
            //m.Faces.ForEach(x => x.Edges.ForEach(x2 => inxs2.AddAll(x2.Indices)));
            LineGeometry3D model = new LineGeometry3D()
            {
                Positions = m.Positions,
                Indices = inxs2,
            };
            LineGeometryModel3D edge = new LineGeometryModel3D() { Geometry = model };
            
            edge.Transform = group;
            edge.Color = Colors.Red;
            return edge;
        }

        private MeshGeometryModel3D VisualFace(BaseModel m)
        {
            m.Update();
            var inxs2 = new IntCollection();
            m.Faces.ForEach(x => inxs2.AddAll(x.Indices));
            MeshGeometry3D model = new MeshGeometry3D()
            {
                Positions = m.Positions,
                Indices = inxs2,
                
            };
            MeshGeometryModel3D edge = new MeshGeometryModel3D() { Geometry = model, Material = PhongMaterials.Black };

            edge.Transform = group;
           
            return edge;
        }
        private Geometry3D ToGeometry(BaseModel m)
        {
            Geometry3D model = new MeshGeometry3D()
            {
                Positions = m.Positions,
                Indices = m.Indices,
                Normals = m.Normals,
                TextureCoordinates = null,
                Tangents = null,
                BiTangents = null,

            };
            return model;
        }


        private MeshGeometryModel3D VisualFigure(BaseModel m)
        {
            group = new System.Windows.Media.Media3D.Transform3DGroup();
            m.Update();
            var tmp = ToGeometry(m);
           
            MeshGeometryModel3D model = new MeshGeometryModel3D() { Geometry = tmp  };
            
            var scale = new System.Windows.Media.Media3D.ScaleTransform3D(5, 5, 5);
            var translate = new System.Windows.Media.Media3D.TranslateTransform3D(rnd.NextDouble(-20, 20), rnd.NextDouble(0, 15), rnd.NextDouble(-20, 20));
            
            group.Children.Add(scale);
            group.Children.Add(translate);
            model.Transform = group;          
            var material = materials[rnd.Next(0, materials.Count - 1)];
            material.DiffuseColor = new Color4(material.DiffuseColor.ToVector3(), 0.5f);
            model.Material = material;
            model.CullMode = CullMode.Back;
            model.IsTransparent = true;

            return model;
        }
    }


    //public class Models
    //{
       
    //    private PhongMaterialCollection materials = new PhongMaterialCollection();
    //    private Random rnd = new Random();
        
    //    private Geometry3D ToGeometry(BaseModel m)
    //    {
    //        Geometry3D model = new MeshGeometry3D()
    //        {
    //            Positions = m.Positions,
    //            Indices = m.Indices,
    //            Normals = m.Normals,
    //            TextureCoordinates = null,
    //            Tangents = null,
    //            BiTangents = null,
                
    //        };
    //        return model;
    //    }
      

    //    public MeshGeometryModel3D VisualFigure(BaseModel m /*int idx*/)
    //    {
    //        var tmp = ToGeometry(m);
    //        MeshGeometryModel3D model = new MeshGeometryModel3D() { Geometry = tmp, CullMode = SharpDX.Direct3D11.CullMode.Back };
    //        var scale = new System.Windows.Media.Media3D.ScaleTransform3D(5, 5, 5);
    //        var translate = new System.Windows.Media.Media3D.TranslateTransform3D(rnd.NextDouble(-20, 20), rnd.NextDouble(0, 15), rnd.NextDouble(-20, 20));
    //        var group = new System.Windows.Media.Media3D.Transform3DGroup();
    //        group.Children.Add(scale);
    //        group.Children.Add(translate);
    //        model.Transform = group;
    //        var material = materials[rnd.Next(0, materials.Count - 1)];
    //        model.Material = material;
    //        return model;
    //    }

        //public MeshNode GetSceneNodeRandom()
        //{
        //    var idx = rnd.Next(0, models.Count);
        //    MeshNode model = new MeshNode() { Geometry = models[idx], CullMode = SharpDX.Direct3D11.CullMode.Back };
        //    var scale = SharpDX.Matrix.Scaling((float)rnd.NextDouble(1, 5), (float)rnd.NextDouble(1, 5), (float)rnd.NextDouble(1, 5));
        //    var translate = SharpDX.Matrix.Translation((float)rnd.NextDouble(-20, 20), (float)rnd.NextDouble(0, 15), (float)rnd.NextDouble(-20, 20));
        //    model.ModelMatrix = scale * translate;
        //    var material = materials[rnd.Next(0, materials.Count - 1)];
        //    model.Material = material;
        //    if (material.DiffuseColor.Alpha < 1)
        //    {
        //        model.IsTransparent = true;
        //    }
        //    return model;
        //}

    //}
}
