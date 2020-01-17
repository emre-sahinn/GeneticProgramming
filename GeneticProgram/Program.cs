using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace GeneticProgram
{
    class Program
    {
        //ESIK_DEGERI: Hata toplamı eşik değerinin altında olan formülleri almamızı sağlar
        static readonly int ESIK_DEGERI = 200;
        //SELEKSIYON_SAYISI: Hata oranı en yüksek olan kaç kromozom silinecek
        static readonly int SELEKSIYON_SAYISI = 1;
        //AGAC_SAYISI: İlk başta rastgele oluşturulcak olan işlem ağaç sayısı
        static readonly int AGAC_SAYISI = 400;
        //AGAC_UST_SINIR: İlk başta rastgele oluşturulcak olan ağaçların max uzunluğu
        static readonly int AGAC_UST_SINIR = 10;
        //AGAC_ALT_SINIR: İlk başta rastgele oluşturulcak olan ağaçların min uzunluğu
        static readonly int AGAC_ALT_SINIR = 3;
        static readonly int SELEKSIYONLU_ISLEM_SAYISI = 300;
        static readonly int SELEKSIYONSUZ_ISLEM_SAYISI = 100;
        //MAX_CROSSOVER_SAYISI: Kaç kere crossover yapılacağını belirler(Random.next(MAX_CROSSOVER_SAYISI) olarak belirlenir)
        static readonly int MAX_CROSSOVER_SAYISI = 20;
        //MAX_MUTASYON_SAYISI: Kaç kere mutasyon yapılacağını belirler(Random.next(MAX_MUTASYON_SAYISI) olarak belirlenir)
        static readonly int MAX_MUTASYON_SAYISI = 150;

        static readonly double CROSSOVER_OLASILIK = 0.7;
        static readonly double MUTASYON_OLASILIK = 0.3;

        static String[] operand = { "*", "/", "+", "-", "^" };
        static String[] terminal = { "r", "h", "p", "2" };//ağaçların daha kolay bulabilmesi açısından 2 eklenmiştir
        static String[] toplamEleman = { "*", "/", "+", "-", "^", "r", "h", "p"/*, "0", "1"*/, "2"/*, "3", "4", "5", "6", "7", "8", "9" */};
        static List<Tree> treeList = new List<Tree>();
        static int generationNumber = 0;
        static void Main(string[] args)
        {
            /*Gözlem: 
             * Gereğinden fazla sayıda crossover yapmak işleri daha da kötüleştiriyor
             * Parametrelere ince ayar yapmak çok önemli
             */
            Random random = new Random();
            //Genetik Algoritma ile Silindirin Hacmini veya Alanını Bulan Algoritmaya Hoşgeldiniz!
            Console.WriteLine("Welcome to Genetic Algorithm that Finds the Volume or Area of the Cylinder!");
            while (true)
            {
                Console.WriteLine("\n*-*-*-*-*-*-*-*-*-*-*");
                //Hangi işlemi yapmak istiyorsunuz? (1: Silindirin Hacmi, 2: Silindirin Alanı): 
                Console.Write("Which action do you want to do? (1: Volume of Cylinder, 2: Area of Cylinder): ");
                string secim = Console.ReadLine();
                Console.WriteLine("*-*-*-*-*-*-*-*-*-*-*");
                generationNumber = 0;

                if (secim.Equals("1"))
                {
                    treeList.Clear();
                    do
                    {
                        createRandomTree();

                        for (int i = 0; i < SELEKSIYONSUZ_ISLEM_SAYISI; i++)
                        {
                            basariHesapla(0, "hacim");
                            double rand = random.NextDouble();
                            if (rand < CROSSOVER_OLASILIK)
                                Crossover();
                            else
                                Mutate();
                        }

                        for (int i = 0; i < SELEKSIYONLU_ISLEM_SAYISI - 1; i++)
                        {
                            basariHesapla(SELEKSIYON_SAYISI, "hacim");
                            double rand = random.NextDouble();
                            if (rand < CROSSOVER_OLASILIK)
                                Crossover();
                            else
                                Mutate();
                        }

                        burnWeaks();

                        foreach (Tree item in treeList)
                        {
                            Console.WriteLine("{0, -15} => {1, -10}  Chance to Survive: {2, -10}", item.toInfix(), Math.Round(item.result,2), item.basariSansi);
                        }

                        if (treeList.Count == 0)
                        {
                            Console.WriteLine("Calculating..");
                            treeList.Clear();
                            generationNumber = 0;
                        }
                    } while (treeList.Count == 0);
                }
                else if (secim.Equals("2"))
                {
                    treeList.Clear();
                    do
                    {
                        createRandomTree();
                        for (int i = 0; i < SELEKSIYONSUZ_ISLEM_SAYISI - 1; i++)
                        {
                            basariHesapla(0, "alan");
                            double rand = random.NextDouble();
                            Crossover();
                            Mutate();
                        }

                        for (int i = 0; i < SELEKSIYONLU_ISLEM_SAYISI; i++)
                        {
                            basariHesapla(SELEKSIYON_SAYISI, "alan");
                            double rand = random.NextDouble();
                            Crossover();
                            Mutate();
                        }

                        burnWeaks();

                        foreach (Tree item in treeList)
                        {
                            Console.WriteLine("{0, -15} => {1, -10}  Chance to Survive: {2, -10}", item.toInfix(), Math.Round(item.result, 2), item.basariSansi);
                        }

                        if (treeList.Count == 0)
                        {
                            Console.WriteLine("Calculating..");
                            treeList.Clear();
                            generationNumber = 0;
                        }
                    } while (treeList.Count == 0);
                }
            }
        }

        static void burnWeaks()//her şey bittikten sonra hala hayatta kalan zayıflar varsa onları yok eder
        {
            for (int i = 0; i < treeList.Count; i++)
            {
                if (treeList[i].result > ESIK_DEGERI || treeList[i].result == double.NaN || treeList[i].basariSansi == 1)
                {
                    treeList.RemoveAt(i);
                    burnWeaks();
                }
            }
        }

        static void Mutate() //Rastgele tree lerin rastgele node larını rastgele değiştirir
        {
            Random random = new Random();
            int mutasyonSayisi = random.Next(MAX_MUTASYON_SAYISI);
            for (int i = 0; i < MAX_MUTASYON_SAYISI; i++)
            {
                int rtree = random.Next(treeList.Count);
                int hayattaKalmaSansi = random.Next(treeList[rtree].basariSansi + 1);
                if (hayattaKalmaSansi != 0)//0 gelirse mutasyona maruz bırak
                {
                    mutasyonSayisi--;
                    continue;
                }
                int rtree_node = random.Next(treeList[rtree].getNodeList().Count);
                TreeNode rNode = treeList[rtree].getNodeList()[rtree_node];
                int gelen;
                if (rNode.leftchild == null && rNode.rightchild == null)//yaprak düğüm ise
                {
                    gelen = random.Next(5, 9);//rastgele terminal data
                    rNode.data = toplamEleman[gelen];
                }
                else //yaprak düğüm değil ise
                {
                    gelen = random.Next(0, 5);//rastgele operand data
                    rNode.data = toplamEleman[gelen];
                }
            }
        }

        static void Crossover()//rastgele seçilen 2 ağaç arasında, rastgele seçilen 2 node un karşılık yer değiştirilmesi
        {
            /*Console.WriteLine("Before:");
            foreach (Tree item in treeList)
            {
                item.inorder(item.root);
                Console.WriteLine("---");
            }
            Console.WriteLine("After:");*/
            Random random = new Random();
            int crossOverSayisi = random.Next(MAX_CROSSOVER_SAYISI);
            for (int i = 0; i < crossOverSayisi; i++)
            {

                int rtree_1 = random.Next(treeList.Count);
                int rtree_2 = random.Next(treeList.Count);
                int hayattaKalmaSansi1 = random.Next(treeList[rtree_1].basariSansi + 1);
                int hayattaKalmaSansi2 = random.Next(treeList[rtree_2].basariSansi + 1);
                if (hayattaKalmaSansi1 != 0)//0 gelirse crossover yaptır
                {
                    crossOverSayisi--;
                    continue;
                }
                if (hayattaKalmaSansi2 != 0)
                {
                    crossOverSayisi--;
                    continue;
                }
                int rtree_1_node = random.Next(treeList[rtree_1].getNodeList().Count);
                int rtree_2_node = random.Next(treeList[rtree_2].getNodeList().Count);
                TreeNode tree_1 = treeList[rtree_1].getNodeList()[rtree_1_node];
                TreeNode tree_2 = treeList[rtree_2].getNodeList()[rtree_2_node];

                TreeNode temp_tree_1 = new TreeNode(tree_1.data, tree_1.leftchild, tree_1.rightchild);
                temp_tree_1.parent = tree_1.parent;
                temp_tree_1.cocukYon = tree_1.cocukYon;
                TreeNode temp_tree_2 = new TreeNode(tree_2.data, tree_2.leftchild, tree_2.rightchild);
                temp_tree_2.parent = tree_2.parent;
                temp_tree_2.cocukYon = tree_2.cocukYon;

                //Node ların karşılıklı yer değiştirmesi
                if (tree_1.cocukYon == 0)
                {
                    tree_1.parent.leftchild = tree_2;
                    TreeNode temp = tree_1.parent;
                    if (tree_2.cocukYon == 0)
                    {
                        tree_2.parent.leftchild = tree_1;
                        tree_1.cocukYon = 0;
                        tree_1.parent = tree_2.parent;
                    }
                    else
                    {
                        tree_2.parent.rightchild = tree_1;
                        tree_1.cocukYon = 1;
                        tree_1.parent = tree_2.parent;
                    }
                    tree_2.parent = temp;
                    tree_2.cocukYon = 0;
                }
                else if (tree_1.cocukYon == 1)
                {
                    tree_1.parent.rightchild = tree_2;
                    TreeNode temp = tree_1.parent;
                    if (tree_2.cocukYon == 0)
                    {
                        tree_2.parent.leftchild = tree_1;
                        tree_1.cocukYon = 0;
                        tree_1.parent = tree_2.parent;
                    }
                    else
                    {
                        tree_2.parent.rightchild = tree_1;
                        tree_1.cocukYon = 1;
                        tree_1.parent = tree_2.parent;
                    }
                    tree_2.parent = temp;
                    tree_2.cocukYon = 1;
                }

            }
            /*foreach (Tree item in treeList)
            {
                item.inorder(item.root);
                Console.WriteLine("---");
            }*/
        }

        static int[,] testData1 =
                { //Silindirin hacmi için: p*(r^2)*h
                {1,2,3*1*2},//r,h,sonuc
                {4,3,3*16*3},
                {2,2,3*4*2},
                {1,5,3*1*5},
                {8,4,3*64*4},
                {3,7,3*9*7},
                {5,9,3*25*9},
                {9,7,3*81*7},
                {4,3,3*16*3},
                {2,5,3*4*5}
            };
        static int[,] testData2 =
                { //Silindirin alanı için: 2*p*r*h
                {1,2,2*3*1*2},//r,h,sonuc
                {4,3,2*3*4*3},
                {2,2,2*3*2*2},
                {1,5,2*3*1*5},
                {8,4,2*3*8*4},
                {3,7,2*3*3*7},
                {5,9,2*3*5*9},
                {9,7,2*3*9*7},
                {4,3,2*3*4*3},
                {2,5,2*3*2*5}
            };
        static void basariHesapla(int silinecekSayi, string type)
        {
            //PI 3 kabul edilmiştir!
            //Silindirin hacmi: p*(r^2)*h = p*r*r*h
            //Silindirin alanı: 2*p*r*h = p*r*h+h = p+p*r*h = p*r+r*h hepsi aynı anlama geliyor(parantezleme olmadığı için)
            generationNumber++;
            Random random = new Random();
            List<double> results = new List<double>();
            foreach (Tree gelen in treeList)
            {
                double toplamHata = 0;
                string infix = gelen.toInfix();
                //Console.WriteLine("infix: " + infix + " root: " + gelen.root.data);
                for (int counter = 0; counter < 10; counter++)
                {
                    int p = 3;
                    int r, h;
                    if (type.Equals("hacim"))
                    {
                        r = testData1[counter, 0];
                        h = testData1[counter, 1];
                    }
                    else
                    {
                        r = testData2[counter, 0];
                        h = testData2[counter, 1];
                    }
                    double[] veriler = { p, r, h };
                    double calculation = gelen.computeValue(gelen.root, veriler);
                    if (type.Equals("hacim"))
                    {
                        //Console.WriteLine("Calculation: " + calculation + " Real value: " + testData1[counter, 2]);
                        toplamHata += Math.Abs(testData1[counter, 2] - calculation);
                    }
                    else
                    {
                        //Console.WriteLine("Calculation: " + calculation + " Real value: " + testData2[counter, 2]);
                        toplamHata += Math.Abs(testData2[counter, 2] - calculation);
                    }

                }
                gelen.result = toplamHata;
                if (toplamHata < ESIK_DEGERI)
                {
                    gelen.basariSansi += Convert.ToInt32(Math.Sqrt(generationNumber));
                }
            }
            for (int i = 0; i < silinecekSayi; i++)
            {
                int rand = random.Next(treeList.Count);
                Tree gelen = treeList[rand];
                int hayattaKalmaSansi = random.Next(gelen.basariSansi + 1);
                if (hayattaKalmaSansi == 0)//0 gelirse öldür
                {
                    treeList.RemoveAt(rand);
                }
            }

            //Console.WriteLine();
            //Console.WriteLine("Best of " + generationNumber + ". generation: " + treeList[results.IndexOf(results.Min())].toInfix() + "\t=>\t" + results.Min());

        }

        //Rastgele işlem ağaçları oluşturup treeList'e ekler
        static void createRandomTree()
        {
            generationNumber = 0;
            Random random = new Random();
            while (treeList.Count < AGAC_SAYISI)
            {
                Tree tree = new Tree();
                do
                {
                    if (tree.size() > AGAC_UST_SINIR)
                        break;
                    int gelen = random.Next(0, 9);
                    tree.insert(toplamEleman[gelen], tree.root);
                    tree.durdurayimMi = true;
                    tree.eklemeyiDurdur(tree.root);
                    if (tree.durdurayimMi && tree.size() > AGAC_ALT_SINIR)
                    {
                        treeList.Add(tree);
                    }

                }
                while (!tree.durdurayimMi);
            }
        }

    }
}