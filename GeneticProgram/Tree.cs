using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticProgram
{
    class Tree
    {
        public int basariSansi = 1;
        public double result;
        public TreeNode root;
        //private List<TreeNode> liste = new List<TreeNode>();

        public Tree()
        {
            root = null;
        }

        Random random = new Random();

        //Gelen random harfleri veya sayıları ağaca rastgele yerleştirir
        public void insert(string gelen, TreeNode localRoot)
        {
            if (root == null)
            {
                root = new TreeNode(gelen);
            }
            if (localRoot != null)
            {

                int yon = random.Next(0, 2);

                if (yon == 0)
                    insert(gelen, localRoot.leftchild);
                else
                    insert(gelen, localRoot.rightchild);

                if (!localRoot.data.Equals("p") && !localRoot.data.Equals("h") &&
                    !localRoot.data.Equals("r") /*&& !localRoot.data.Equals("0") &&
                    !localRoot.data.Equals("1")*/ && !localRoot.data.Equals("2") /*&&
                    !localRoot.data.Equals("3") && !localRoot.data.Equals("4") &&
                    !localRoot.data.Equals("5") && !localRoot.data.Equals("6") &&
                    !localRoot.data.Equals("7") && !localRoot.data.Equals("8") &&
                    !localRoot.data.Equals("9")*/)
                {
                    if (localRoot.leftchild != null && localRoot.rightchild == null)
                    {
                        //if(localRoot.data.Equals("p") || localRoot.data.Equals("r") || localRoot.data.Equals("h"))
                        localRoot.rightchild = new TreeNode(gelen);
                        localRoot.rightchild.parent = localRoot;
                        localRoot.rightchild.cocukYon = 1;
                    }
                    else if (localRoot.leftchild == null && localRoot.rightchild != null)
                    {
                        localRoot.leftchild = new TreeNode(gelen);
                        localRoot.leftchild.parent = localRoot;
                        localRoot.leftchild.cocukYon = 0;
                    }
                    else if (localRoot.leftchild == null && localRoot.rightchild == null)
                    {
                        int randomDeger = random.Next(0, 2);
                        if (randomDeger == 0)
                        {
                            localRoot.leftchild = new TreeNode(gelen);
                            localRoot.leftchild.parent = localRoot;
                            localRoot.leftchild.cocukYon = 0;
                        }
                        else
                        {
                            localRoot.rightchild = new TreeNode(gelen);
                            localRoot.rightchild.parent = localRoot;
                            localRoot.rightchild.cocukYon = 1;
                        }
                    }
                }
            }
        }


        private double işlem(string işlemci, double l, double r)
        {
            double sonuç = 0;
            //if (işlemci.Equals("/") && r == 0)//0 a bölme hatası yaparsa hata oranını çok yüksek ver
            //    return 999;
            switch (işlemci)
            {
                case "^": sonuç = Math.Pow(l, r); break;
                case "*": sonuç = l * r; break;
                case "/": sonuç = l / r; break;
                case "+": sonuç = l + r; break;
                case "-": sonuç = l - r; break;
            }
            return sonuç;
        }
        public double computeValue(TreeNode localRoot, double[] gelen = null)
        {
            if (localRoot == null) return 0;
            if (localRoot.leftchild == null && localRoot.rightchild == null)
            {
                if(gelen != null)
                {
                    if (localRoot.data.Equals("p"))
                        return gelen[0];
                    else if (localRoot.data.Equals("r"))
                        return gelen[1];
                    else if (localRoot.data.Equals("h"))
                        return gelen[2];
                    else
                        return Double.Parse(localRoot.data);
                }
                else
                    return Double.Parse(localRoot.data);
            }

            string işaret = localRoot.data;
            double lValue = computeValue(localRoot.leftchild, gelen);
            double rValue = computeValue(localRoot.rightchild, gelen);
            return işlem(işaret, lValue, rValue);
        }

        List<TreeNode> nodeList = new List<TreeNode>();
        private void ConvertToNodeList(TreeNode localRoot)//Ağacı node listesine dönüştürür(crossover ve mutasyonda işe yarıyor)
        {
            if (localRoot != null)
            {
                ConvertToNodeList(localRoot.leftchild);
                if (localRoot != root)
                    nodeList.Add(localRoot);
                ConvertToNodeList(localRoot.rightchild);
            }
        }
        public List<TreeNode> getNodeList()
        {
            nodeList.Clear();
            ConvertToNodeList(root);
            return nodeList;
        }


        private string infix = "";
        private void convertToInfix(TreeNode localRoot)
        {
            if (localRoot != null)
            {
                convertToInfix(localRoot.leftchild);
                infix += localRoot.data;
                convertToInfix(localRoot.rightchild);
            }
        }
        public string toInfix()
        {
            infix = "";
            convertToInfix(root);
            return infix;
        }


        public bool durdurayimMi = true;
        public void eklemeyiDurdur(TreeNode localroot)//Ağaç dolduysa eklemeyi durdurmayı sağlayan metod
        {
            if (localroot != null)
            {
                eklemeyiDurdur(localroot.leftchild);

                if (localroot.leftchild != null &&
                    (localroot.leftchild.data.Equals("p") || localroot.leftchild.data.Equals("r") ||
                    localroot.leftchild.data.Equals("h")/* || localroot.leftchild.data.Equals("0") ||
                    localroot.leftchild.data.Equals("1") */|| localroot.leftchild.data.Equals("2") /*||
                    localroot.leftchild.data.Equals("3") || localroot.leftchild.data.Equals("4") ||
                    localroot.leftchild.data.Equals("5") || localroot.leftchild.data.Equals("6") ||
                    localroot.leftchild.data.Equals("7") || localroot.leftchild.data.Equals("8") ||
                    localroot.leftchild.data.Equals("9")*/) && localroot.rightchild == null)
                {
                    durdurayimMi = false;
                }
                else if (localroot.rightchild != null &&
                   (localroot.rightchild.data.Equals("p") || localroot.rightchild.data.Equals("r") ||
                   localroot.rightchild.data.Equals("h")/* || localroot.rightchild.data.Equals("0") ||
                   localroot.rightchild.data.Equals("1") */|| localroot.rightchild.data.Equals("2")/* ||
                   localroot.rightchild.data.Equals("3") || localroot.rightchild.data.Equals("4") ||
                   localroot.rightchild.data.Equals("5") || localroot.rightchild.data.Equals("6") ||
                   localroot.rightchild.data.Equals("7") || localroot.rightchild.data.Equals("8") ||
                   localroot.rightchild.data.Equals("9")*/)
                   && localroot.leftchild == null)
                {
                    durdurayimMi = false;
                }
                else if (localroot.leftchild == null && localroot.rightchild == null)
                {
                    if (localroot.data.Equals("*") || localroot.data.Equals("/") || localroot.data.Equals("+") || localroot.data.Equals("-") || localroot.data.Equals("^"))
                    {
                        durdurayimMi = false;
                    }

                }
                //localroot.displayNode();
                eklemeyiDurdur(localroot.rightchild);
            }
        }

        public void preorder(TreeNode localroot)
        {
            if (localroot != null)
            {
                localroot.displayNode();
                preorder(localroot.leftchild);
                preorder(localroot.rightchild);
            }
        }
        public void inorder(TreeNode localroot)
        {
            if (localroot != null)
            {

                inorder(localroot.leftchild);
                localroot.displayNode();
                inorder(localroot.rightchild);
            }
        }
        public void postorder(TreeNode localroot)
        {
            if (localroot != null)
            {

                postorder(localroot.leftchild);
                postorder(localroot.rightchild);
                localroot.displayNode();
            }
        }
        private int sizeCount;
        public void findSize(TreeNode localroot)
        {
            if (localroot != null)
            {
                findSize(localroot.leftchild);
                sizeCount++;
                findSize(localroot.rightchild);
            }
        }

        public int size()
        {
            sizeCount = 0;
            findSize(root);
            return sizeCount;
        }
    }

    class TreeNode
    {
        public String data;
        public TreeNode leftchild;
        public TreeNode rightchild;
        public TreeNode parent;
        public int cocukYon;

        public TreeNode(String data, TreeNode leftchild = null, TreeNode rightchild = null)
        {
            this.data = data;
            this.leftchild = leftchild;
            this.rightchild = rightchild;
        }

        public void displayNode()
        {
            Console.WriteLine(" " + data + " ");
        }
    }

}
