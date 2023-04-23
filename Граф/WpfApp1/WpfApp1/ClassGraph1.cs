using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    public class ClassGraph1 : DependencyObject
    {
        public ObservableCollection<Node> Nodes { get; private set; }
        public ObservableCollection<Edge> Edges { get; private set; }
        public int nextIndex;
        public ClassGraph1()
        {
            this.Nodes = new ObservableCollection<Node>();
            this.Edges = new ObservableCollection<Edge>();        
            this.nextIndex = 1;           
        }
        public Node AddNode(double x, double y)
        {
            Node node = new Node(nextIndex, x, y);
            this.Nodes.Add(node);
            nextIndex++;
            return node;
        }
        public void AddEdge(Node startNode, Node endNode)
        {
            this.Edges.Add(new Edge(startNode, endNode));
        }
        public int[,] BuildAdjacencyMatrix(ObservableCollection<Node> nodes, ObservableCollection<Edge> edges)
        {
            // Создаем матрицу размером равным числу узлов
            int[,] adjacencyMatrix = new int[nodes.Count, nodes.Count];
            // Заполняем матрицу нулями
            for (int i = 0; i < nodes.Count; i++)
            {
                for (int j = 0; j < nodes.Count; j++)
                {
                    adjacencyMatrix[i, j] = 0;
                }
            }
            // Заполняем матрицу единицами в соответствующих ячейках, где есть связь между узлами
            foreach (Edge edge in edges)
            {
                    int startNodeIndex = nodes.IndexOf(edge.StartNode);
                    int endNodeIndex = nodes.IndexOf(edge.EndNode);
                    adjacencyMatrix[startNodeIndex, endNodeIndex] = 1;
                    adjacencyMatrix[endNodeIndex, startNodeIndex] = 1;       
            }
            return adjacencyMatrix;
        }

        public void Clear()
        {
            this.Nodes = null;
            this.Edges = null;
            this.nextIndex = 1;
        }
    }
    public class Edge : DependencyObject
    {
        public double startNode, endNode;
        public Node StartNode
        {
            get { return (Node)GetValue(StartNodeProperty); }
            set { SetValue(StartNodeProperty, value); }
        }
        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartNodeProperty =
            DependencyProperty.Register("StartNode", typeof(Node), typeof(Edge), new PropertyMetadata(null));
        public Node EndNode
        {
            get { return (Node)GetValue(EndNodeProperty); }
            set { SetValue(EndNodeProperty, value); }
        }
        // Using a DependencyProperty as the backing store for EndNote.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndNodeProperty =
            DependencyProperty.Register("EndNode", typeof(Node), typeof(Edge), new PropertyMetadata(null));
        public Edge(Node startNode, Node endNode)
        {
            this.StartNode = startNode;
            this.EndNode = endNode;
        }
    }
    public class Node : DependencyObject
    {
        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Index.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(Node), new PropertyMetadata(0));
        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }
        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register("X", typeof(double), typeof(Node), new PropertyMetadata(0.0));
        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Y.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register("Y", typeof(double), typeof(Node), new PropertyMetadata(0.0));
        public double CentreX
        {
            get { return (double)GetValue(CentreXProperty); }
            set { SetValue(CentreXProperty, value); }
        }
        // Using a DependencyProperty as the backing store for CentrX.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CentreXProperty =
            DependencyProperty.Register("CentreX", typeof(double), typeof(Node), new PropertyMetadata(0.0));
        public double CentreY
        {
            get { return (double)GetValue(CentreYProperty); }
            set { SetValue(CentreYProperty, value); }
        }
        // Using a DependencyProperty as the backing store for CentrY.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CentreYProperty =
            DependencyProperty.Register("CentreY", typeof(double), typeof(Node), new PropertyMetadata(0.0));
        public double Size
        {
            get { return (double)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }
        // Using a DependencyProperty as the backing store for Size.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register("Size", typeof(double), typeof(Node), new PropertyMetadata(0.0));
        public Node(int index, double centerX, double centerY)
        {
            this.Index = index;
            this.CentreX = centerX;
            this.CentreY = centerY;
            this.Size = 50;
            this.X = this.CentreX - this.Size / 2;
            this.Y = this.CentreY - this.Size / 2;
        }
    }
}
