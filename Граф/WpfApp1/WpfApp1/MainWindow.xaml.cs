using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        private ClassGraph1 Graph;
        private Canvas canvasNodes;

        
        public MainWindow()
        {
            InitializeComponent();
            Graph = new ClassGraph1();
          
            Node nod1 = Graph.AddNode(50, 400);
            Node nod2 = Graph.AddNode(150, 200);
            Node nod3 = Graph.AddNode(300, 40);

            Graph.AddEdge(nod1, nod2);
            Graph.AddEdge(nod3, nod2);


        
            ListBoxNodes.ItemsSource = Graph.Nodes;
            ListBoxEdges.ItemsSource = Graph.Edges;

           

        }


      



        //проверка спауна
        private void canvasNodes_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Canvas canvasNodes = sender as Canvas;
                if (canvasNodes != null)
                {
                    Point point = e.GetPosition(canvasNodes);
                    Graph.AddNode(point.X, point.Y);
                }
            }
            catch (Exception ex)
            {

            }
        }



        private bool isDraggingNode = false;
        private Point? previousPoint = null;
        private Node firstNode = null;

        List<int> Svyzi = new List<int> { };
        //спаун связей и узлов
        private void gridNode_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Grid grid = sender as Grid;
            if (grid != null)
            {
                grid.CaptureMouse();

                if (radioAddNode.IsChecked == true)
                {
                    isDraggingNode = true;
                    previousPoint = e.GetPosition(canvasNodes);
                }

                if (radioAddEdge.IsChecked == true)
                {
                    Svyzi.Add(1);
                    Node currentNode = null;
                    currentNode = grid.DataContext as Node;
                    if (firstNode == null)
                    {
                        
                        firstNode = currentNode;
                        
                    }
                    else
                    {
                        Svyzi.Add(0);
                        Graph.AddEdge(firstNode, currentNode);
                        firstNode = null;
                    }

                }
            }
        }



        // передвижение точек
        private void gridNode_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDraggingNode)
            {
                Grid element = sender as Grid;
                if (element != null)
                {
                    Node node = element.DataContext as Node;
                    
                    if (node != null && previousPoint.HasValue)
                    {
                        Point point = e.GetPosition(canvasNodes);

                        double xDiff = point.X - previousPoint.Value.X;
                        double yDiff = point.Y - previousPoint.Value.Y;

                        node.X += xDiff;
                        node.Y += yDiff;

                        node.CentreX += xDiff;
                        node.CentreY += yDiff;

                        previousPoint = point;
                    }
                }
              
            }
        }

        private void gridNode_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Grid grid = sender as Grid;
            if (grid != null)
            {
                grid.ReleaseMouseCapture();
                isDraggingNode = false;
                previousPoint = null;
            }
        }

       private void canvasNodes_Loaded(object sender, RoutedEventArgs e)
       {
            canvasNodes = sender as Canvas;
       }    

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            int[,] Table = Graph.BuildAdjacencyMatrix(Graph.Nodes, Graph.Edges);

            List<int> myStrings = new List<int> { };

            for (var i = 0; i < Table.GetLength(0); i++)
            {
                for (var j = 0; j < Table.GetLength(1); j++)
                {
                    myStrings.Add(Table[i, j]);
                }
            }

            int uzel = 1, uzel_uzel = 1, end = (Graph.nextIndex - 1) * 2, start = 0;
            string matrix = string.Empty, uzel1point = string.Empty, myIntsString = string.Join(" ", myStrings) + " ";
         
            uzel1point += "   | ";
            for (var i = 0; i < Graph.nextIndex - 1; i++)
            {               
                uzel1point += $"{uzel_uzel} ";
                uzel_uzel++;
            }

            for (var i = 0; i < Graph.nextIndex - 1; i++)
            {
                matrix += $"{uzel} | " + myIntsString.Substring(start, end) + "\n";
                start += end;
                uzel++;
            }
            uzel1point += "\n";

            myTextBox.Text = uzel1point + matrix;
           
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            string myIntsString = string.Empty;

            if (pathStart() == false || pathEnd() == false)
            {
                myText.Background  = Brushes.Red;
            }
            else
            {
                myText.Background = Brushes.Transparent;
                myIntsString += "Кол-во узлов:\n";
                myIntsString += string.Join(" ", path(Graph.BuildAdjacencyMatrix(Graph.Nodes, Graph.Edges)) + "\n");
                myIntsString += "Номера узлов:\n";
                myIntsString += indexStart.Text.Trim() + " -> ";
                myIntsString += indexEnd.Text.Trim() + "\n";
            }

               myText.Text = myIntsString;


        }
        //алгоритм Дейкстры для поиска мин кол-ва связей между двумя узлами
        public int path(int[,] matrix)
        {
            int start, end;

            if (pathStart() == false || pathEnd() == false)
            {
                myText.Background = Brushes.Red;
                start = 0;
                end = 0;
            }
            else if(Convert.ToInt32(indexStart.Text.Trim()) < 1 || Convert.ToInt32(indexStart.Text.Trim()) > matrix.GetLength(0) || Convert.ToInt32(indexEnd.Text.Trim()) > matrix.GetLength(0))
            {
                myText.Background = Brushes.Red;
                start = 0;
                end = 0;
            }
            else
            {
                myText.Background = Brushes.Transparent;
                start = Convert.ToInt32(indexStart.Text.Trim())-1;
                end = Convert.ToInt32(indexEnd.Text.Trim())-1;

            }


            int[] distance = new int[matrix.GetLength(0)];
            bool[] visited = new bool[matrix.GetLength(0)];
            for (int i = 0; i < distance.Length; i++)
            {
                distance[i] = int.MaxValue;
            }
            distance[start] = 0;
            for (int i = 0; i < distance.Length - 1; i++)
            {
                int minIndex = -1;
                for (int j = 0; j < distance.Length; j++)
                {
                    if (!visited[j] && (minIndex == -1 || distance[j] < distance[minIndex]))
                    {
                        minIndex = j;
                    }
                }
                visited[minIndex] = true;
                for (int j = 0; j < matrix.GetLength(0); j++)
                {
                    if (matrix[minIndex, j] != 0 && distance[minIndex] != int.MaxValue && distance[minIndex] + matrix[minIndex, j] < distance[j])
                    {
                        distance[j] = distance[minIndex] + matrix[minIndex, j];
                    }
                }
            }
           
            return distance[end];
        }

  

        public bool pathStart()
        {
           
            string Start = indexStart.Text.Trim();

            if (!Regex.IsMatch(Start, "^[0-9]+$"))
            {
                indexStart.Background = Brushes.IndianRed;
                return false;
            }
            else
            {
                indexStart.Background = Brushes.Transparent;
                return true;
            }
          
        }
        public bool pathEnd()
        {         
            string End = indexEnd.Text.Trim();

            if (!Regex.IsMatch(End, "^[0-9]+$")) 
            {
                indexEnd.Background = Brushes.IndianRed;
                return false;
            }
            else
            {
                indexEnd.Background = Brushes.Transparent;
                return true;

            }

        }

        //алгоритм для обхода в глубь графа

        private bool[] visited = new bool[1000];
         
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            
            for (int i = 0; i < visited.Length; i++)
            {
                visited[i] = false;
            }
            listBox1.Items.Clear();
            DFS(0);

        }

        private void DFS(int i)
        {
            int[,] adjMatrix = Graph.BuildAdjacencyMatrix(Graph.Nodes, Graph.Edges);
            visited[i] = true;
            listBox1.Items.Add("Узел: " + (i+1).ToString());
            for (int j = 0; j < adjMatrix.GetLength(1); j++)
            {
                if (adjMatrix[i, j] == 1 && !visited[j])
                {
                    DFS(j);
                }
            }
        }


        // Определение циклов

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            int[,] adjacencyMatrix = Graph.BuildAdjacencyMatrix(Graph.Nodes, Graph.Edges);

           

            myTextBox1.Text = string.Empty;

            // Эйлеров цикл
             
            
            bool euler = true;
            for (int i = 0; i < adjacencyMatrix.GetLength(0); i++)
            {
                int degree = 0;
                for (int j = 0; j < adjacencyMatrix.GetLength(1); j++)
                {
                    degree += adjacencyMatrix[i, j];
                }
                if (degree % 2 != 0)
                {
                    euler = false;
                    break;
                }
            }

            if (euler)
            {
                myTextBox1.Text += "Граф содержит \nэйлеров цикл.\n\n";
            }
            else
            {
                myTextBox1.Text += "Граф не содержит \nэйлеров цикл.\n\n";
            }


            // Гамлетов цикл

            if (HamiltonianCycle(adjacencyMatrix))
            {
                myTextBox1.Text += "Граф содержит \nгамильтонов цикл.\n\n";
            }
            else
            {
                myTextBox1.Text += "Граф не содержит \nгамильтонов цикл.\n\n";
            }






        }

         bool HamiltonianCycle(int[,] graph)
         {
            int[] path = new int[graph.GetLength(0)];
            for (int i = 0; i < path.Length; i++)
            {
                path[i] = -1;
            }
            path[0] = 0;
            if (!HamiltonianCycleUtil(graph, path, 1))
            {
                return false;
            }
            return true;
         }

         bool HamiltonianCycleUtil(int[,] graph, int[] path, int pos)
         {
            if (pos == graph.GetLength(0))
            {
                if (graph[path[pos - 1], path[0]] == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            for (int v = 1; v < graph.GetLength(0); v++)
            {
                if (IsSafe(v, graph, path, pos))
                {
                    path[pos] = v;
                    if (HamiltonianCycleUtil(graph, path, pos + 1))
                    {
                        return true;
                    }
                    path[pos] = -1;
                }
            }
            return false;
         }



        static bool IsSafe(int v, int[,] graph, int[] path, int pos)
        {
            if (graph[path[pos - 1], v] == 0)
            {
                return false;
            }
            for (int i = 0; i < pos; i++)
            {
                if (path[i] == v)
                {
                    return false;
                }
            }
            return true;
        }


        // очистка
        void Button_Click_4(object sender, RoutedEventArgs e)
        {
            Graph.Nodes.Clear();
            Graph.Edges.Clear();
            myTextBox1.Text = string.Empty;
            myText.Text = string.Empty;
            myTextBox.Text = string.Empty;

          Graph = null;

            InitializeComponent();
            Graph = new ClassGraph1();
            ListBoxNodes.ItemsSource = Graph.Nodes;
            ListBoxEdges.ItemsSource = Graph.Edges;

        }
    }

}
