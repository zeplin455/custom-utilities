using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ZUtils.DataStructs;
using ZUtils.DataStructs.DirectDictionary;
using ZUtils.DataStructs.DirectDictionary.Containers;
using ZUtils.Utilities;
using ZUtils.DataStructs.Looping;
using ZUtils.DataStructs.Looping.FuzzyComparers;

namespace ZUtils
{
    class Program
    {
        static void Main(string[] args)
        {
            TestNineTree();

            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }

        private static void TestNineTree()
        {
            Ninetree<int> tree = new Ninetree<int>();

            tree.Add(455, int.MaxValue, int.MaxValue, int.MaxValue);
            tree.Add(455, -1, 0, 1);
            tree.Add(455, 1, 0, -1);

            Random rnd = new Random();
            for (int i = 0; i < 10000; ++i)
            {
                int val = rnd.Next();
                int X = rnd.Next(0, int.MaxValue) - (int.MaxValue / 2);
                int Y = rnd.Next(0, int.MaxValue) - (int.MaxValue / 2);
                int Z = rnd.Next(0, int.MaxValue) - (int.MaxValue / 2);
                tree.Add(val, X, Y, Z);
                int result = tree.Get(X, Y, Z);

                if (result != val)
                {
                    throw new Exception("ERROR, ERROR");
                }
            }

            int v1 = tree.Get(0, 0, 0);
            int v2 = tree.Get(-1, 0, 1);
            int v3 = tree.Get(1, 0, -1);
        }

        /// <summary>
        /// An example of some of the ways data can be stored and retrieved from DirectDictionaryLayer
        /// </summary>
        private static void GetAndSet()
        {
            DirectDictionaryLayerSimple<int> layer = new DirectDictionaryLayerSimple<int>();

            layer["123"] = 1;
            layer["123"]["456"] = 2;
            layer.Set("1234-567".ConvertToNibbles(), 3);
            layer[(int)2000] = 5;
            layer[(long)2000] = 6;
            layer['A']['B'] = 7;


            int four = layer.GetOrSet("1234-567-89".ConvertToNibbles(), () => { return 4; });

            Console.WriteLine(layer["123"]);
            Console.WriteLine(layer["1"]["23"]);

            Console.WriteLine(layer["123456"]);
            Console.WriteLine(layer["123"]["456"]);

            Console.WriteLine(layer["1234-567"]);
            Console.WriteLine(layer["1234"]["-"]["567"]);

            Console.WriteLine(four);
            Console.WriteLine(layer["1234-567-89"]);

            Console.WriteLine(layer[(int)2000]);
            Console.WriteLine(layer[(long)2000]);
            Console.WriteLine(layer['A']['B']);
            Console.WriteLine(layer.Get("123456"));
        }

        /// <summary>
        /// Example of how you can filter when keys follow a hierarchical pattern
        /// </summary>
        private static void QuickFilter()
        {
            DirectDictionaryLayerSimple<int> layer = new DirectDictionaryLayerSimple<int>();

            layer["--1--A--A1"] = 1;
            layer["--1--A--A2"] = 2;
            layer["--1--A--A3"] = 3;
            layer["--1--B--A1"] = 4;
            layer["--1--B--A2"] = 5;
            layer["--1--B--A3"] = 6;
            layer["--1--B--A4"] = 7;


            DirectDictionaryLayer<int, SimpleContainer<int>> subLayerA = layer.Filter("--1--A".ConvertToNibbles());
            DirectDictionaryLayer<int, SimpleContainer<int>> subLayerB = layer.Filter("--1--B".ConvertToNibbles());

            Console.WriteLine("Part A");
            subLayerA.ExecuteOnAllItems((v, k) => {
                Console.WriteLine($"Value [{v}] at SubKey [{k.ConvertFromFromNibbles()}]");
            });


            Console.WriteLine("Part B");
            subLayerB.ExecuteOnAllItems((v, k) => {
                Console.WriteLine($"Value [{v}] at SubKey [{k.ConvertFromFromNibbles()}]");
            });
        }


        /// <summary>
        /// Example of how DirectDictionaryLayer can loop around on itself when you set up to do so
        /// </summary>
        private static void LoopAround()
        {
            DirectDictionaryLayerSimple<int> layer = new DirectDictionaryLayerSimple<int>();

            layer.SetLayer("--1".ConvertToNibbles(), layer, false);
            layer.SetLayer("---2".ConvertToNibbles(), layer, false);

            layer["--1"] = 1;
            layer["---2"] = 2;

            Console.WriteLine(layer["--1"]);
            Console.WriteLine(layer["--1--1"]);
            Console.WriteLine(layer["--1--1--1"]);

            Console.WriteLine(layer["--1--1--1---2---2--1"]);

            Console.WriteLine(layer["---2"]);
            Console.WriteLine(layer["---2---2"]);
            Console.WriteLine(layer["---2---2---2"]);

            Console.WriteLine(layer["---2---2--1--1---2--1---2"]);

        }

        class AnimalProperty
        {
            public string AnimalName { get; set; }
            public string AnimalFeature { get; set; }
        }

        /// <summary>
        /// Example of how DirectDictionaryLayer can be use for grouping
        /// </summary>
        private static void Grouping()
        {
            List<AnimalProperty> mixedAnimalFeatures = new List<AnimalProperty>()
            {
                new AnimalProperty(){ AnimalName = "horse", AnimalFeature = "kicks people" },
                new AnimalProperty(){ AnimalName = "dog", AnimalFeature = "excited" },
                new AnimalProperty(){ AnimalName = "cat", AnimalFeature = "loveable" },
                new AnimalProperty(){ AnimalName = "horse", AnimalFeature = "evil" },
                new AnimalProperty(){ AnimalName = "dog", AnimalFeature = "barks" },
                new AnimalProperty(){ AnimalName = "dog", AnimalFeature = "protective" },
                new AnimalProperty(){ AnimalName = "cat", AnimalFeature = "soft" },
                new AnimalProperty(){ AnimalName = "horse", AnimalFeature = "conducts psychological warfare" },
                new AnimalProperty(){ AnimalName = "dog", AnimalFeature = "eats barf" },
                new AnimalProperty(){ AnimalName = "cat", AnimalFeature = "fluffy paws" },
                new AnimalProperty(){ AnimalName = "horse", AnimalFeature = "bites" },
                new AnimalProperty(){ AnimalName = "dog", AnimalFeature = "friendly" }
            };

            DirectDictionaryLayerSimple<List<AnimalProperty>> layer = new DirectDictionaryLayerSimple<List<AnimalProperty>>();

            //Do grouping
            foreach (AnimalProperty animalThing in mixedAnimalFeatures)
            {
                List<AnimalProperty> groupList = layer.GetOrSet(animalThing.AnimalName.ConvertToNibbles(), () => { return new List<AnimalProperty>(); });
                groupList.Add(animalThing);
            }
            //Done grouping

            List<AnimalProperty> dogStuff = layer["dog"];
            List<AnimalProperty> catStuff = layer["cat"];
            List<AnimalProperty> horseStuff = layer["horse"];

            Console.WriteLine("\nDog features:");
            for (int i = 0; i < dogStuff.Count; ++i)
            {
                Console.WriteLine(dogStuff[i].AnimalFeature);
            }
            Console.WriteLine();

            Console.WriteLine("Cat features:");
            for (int i = 0; i < catStuff.Count; ++i)
            {
                Console.WriteLine(catStuff[i].AnimalFeature);
            }
            Console.WriteLine();

            Console.WriteLine("Horse features:");
            for (int i = 0; i < horseStuff.Count; ++i)
            {
                Console.WriteLine(horseStuff[i].AnimalFeature);
            }
            Console.WriteLine();

        }

        private static void Delete()
        {
            DirectDictionaryLayerSimple<int> layer = new DirectDictionaryLayerSimple<int>();
            layer["asd"] = 5;
            layer["asdr4"] = 6;
            layer["bsd"] = 5;
            layer["bsdr4"] = 6;
            layer["csd"] = 5;
            layer["csdr4"] = 6;

            layer.DeleteWithoutShrink("asd".ConvertToNibbles());
            layer.DeleteWithoutShrink("asdr4".ConvertToNibbles());
            layer.DeleteWithoutShrink("bsd".ConvertToNibbles());
            layer.DeleteWithoutShrink("bsdr4".ConvertToNibbles());
            layer.DeleteWithoutShrink("csd".ConvertToNibbles());
            layer.DeleteWithoutShrink("csdr4".ConvertToNibbles());
            layer.Shrink();

        }


        //private static void TestModelComparison()
        //{
        //    Obj ObjFile = new Obj();
        //    ObjFile.LoadObj("Content/Models/monkey.obj");

        //    SortedDictionary<int, SortedSet<int>> VertexFaceMap = new SortedDictionary<int, SortedSet<int>>();
        //    SortedDictionary<int, SortedSet<int>> FaceVertexMap = new SortedDictionary<int, SortedSet<int>>();
        //    SortedDictionary<int, SortedSet<int>> VertexVertexMap = new SortedDictionary<int, SortedSet<int>>();
        //    SortedDictionary<int, SortedSet<VertexDistance>> VertexVertexDistanceMap = new SortedDictionary<int, SortedSet<VertexDistance>>();

        //    for (int i = 0; i < ObjFile.FaceList.Count; ++i)
        //    {
        //        for (int fp = 0; fp < ObjFile.FaceList[i].VertexIndexList.Length; ++fp)
        //        {
        //            if (VertexFaceMap.ContainsKey(ObjFile.FaceList[i].VertexIndexList[fp]))
        //            {
        //                VertexFaceMap[ObjFile.FaceList[i].VertexIndexList[fp]].Add(ObjFile.FaceList[i].Index);
        //            }
        //            else
        //            {
        //                VertexFaceMap.Add(ObjFile.FaceList[i].VertexIndexList[fp], new SortedSet<int>() { ObjFile.FaceList[i].Index });
        //            }

        //            if (FaceVertexMap.ContainsKey(ObjFile.FaceList[i].Index))
        //            {
        //                FaceVertexMap[ObjFile.FaceList[i].Index].Add(ObjFile.FaceList[i].VertexIndexList[fp]);
        //            }
        //            else
        //            {
        //                FaceVertexMap.Add(ObjFile.FaceList[i].Index, new SortedSet<int>() { ObjFile.FaceList[i].VertexIndexList[fp] });
        //            }

        //            if (!VertexVertexMap.ContainsKey(ObjFile.FaceList[i].VertexIndexList[fp]))
        //            {
        //                VertexVertexMap.Add(ObjFile.FaceList[i].VertexIndexList[fp], new SortedSet<int>());
        //                VertexVertexDistanceMap.Add(ObjFile.FaceList[i].VertexIndexList[fp], new SortedSet<VertexDistance>());
        //            }

        //            for (int v = 0; v < ObjFile.FaceList[i].VertexIndexList.Length; ++v)
        //            {
        //                if (ObjFile.FaceList[i].VertexIndexList[fp] != ObjFile.FaceList[i].VertexIndexList[v])
        //                {
        //                    VertexVertexMap[ObjFile.FaceList[i].VertexIndexList[fp]].Add(ObjFile.FaceList[i].VertexIndexList[v]);

        //                    VertexVertexDistanceMap[ObjFile.FaceList[i].VertexIndexList[fp]].Add(new VertexDistance()
        //                    {
        //                        Source = ObjFile.FaceList[i].VertexIndexList[fp],
        //                        Destination = ObjFile.FaceList[i].VertexIndexList[v],
        //                        Distance = Vertex.CalcDistance(ObjFile.VertexList[ObjFile.FaceList[i].VertexIndexList[v] - 1], ObjFile.VertexList[ObjFile.FaceList[i].VertexIndexList[fp] - 1])
        //                    });
        //                }
        //            }
        //        }
        //    }

        //    SortedSet<VertexDistance> Shape = new SortedSet<VertexDistance>();

        //    foreach (int Key in VertexVertexDistanceMap.Keys)
        //    {
        //        foreach (VertexDistance DistObj in VertexVertexDistanceMap[Key])
        //        {
        //            Shape.Add(DistObj);
        //        }
        //    }

        //    List<VertexDistance> ListShape = Shape.ToList();

        //    for (int i = 0; i < ListShape.Count; ++i)
        //    {
        //        ulong CurrentId = ListShape[i].Id;
        //        ListShape[i].ConnectedSide1.AddRange(Shape.Where(x => x.Id != CurrentId && (x.Side1 == ListShape[i].Side1 || x.Side2 == ListShape[i].Side1)).OrderBy(x => x.Distance).ThenBy(x => x.Side1).ThenBy(x => x.Side2));
        //        ListShape[i].ConnectedSide2.AddRange(Shape.Where(x => x.Id != CurrentId && (x.Side1 == ListShape[i].Side2 || x.Side2 == ListShape[i].Side2)).OrderBy(x => x.Distance).ThenBy(x => x.Side1).ThenBy(x => x.Side2));
        //    }

        //    LoopingSequence<VertexDistance> LoopingSequence = new LoopingSequence<VertexDistance>(ListShape.ToArray());
        //}

        private static void TestLoopingSequence()
        {
            Random rnd = new Random();
            byte[] sequence1 = new byte[] { 1, 2, 3, 1, 2, 3, 1, 2, 3 };
            byte[] sequence2 = new byte[] { 10, 1, 2, 3, 1, 2, 3, 1, 2, 3, 10 };
            //byte[] sequence1 = new byte[] { 7, 7, 7, 7, 7, 7, 7, 7, 7 };
            //byte[] sequence2 = new byte[] { 7, 7, 7, 7, 7, 7, 7, 7, 7, 1 };

            //rnd.NextBytes(sequence1);
            //rnd.NextBytes(sequence2);

            Console.WriteLine("Starting");

            LoopingSequence<byte> seq1 = new LoopingSequence<byte>(sequence1);
            LoopingSequence<byte> seq2 = new LoopingSequence<byte>(sequence2);

            List<SequenceMatchScore> scores = seq1.CompareSequence(seq2, new ByteComparer(0), out int ThisBestStartIndex, out int BestMatchScore, out int BestOtherMatchIndex, 3, 3);

            Console.WriteLine($"ThisBestStartIndex : {ThisBestStartIndex}");
            Console.WriteLine($"BestMatchScore : {BestMatchScore}");
            Console.WriteLine($"BestOtherMatchIndex : {BestOtherMatchIndex}");
        }

        
    }
}
