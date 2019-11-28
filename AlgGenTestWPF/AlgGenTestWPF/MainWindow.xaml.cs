using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AlgGenTestWPF
{
    /// <summary> 
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer renderTimer;
        public MainWindow()
        {
            InitializeComponent();
            InitAll();
            Test();
        }

        private void Test()
        {
            listbox.Items.Clear();
            canvas.Children.Clear();
            Engine.tick = 0;
            Engine.scale = 1;
            Engine.shape = null;
            resultLabel.Content = "Getting new Population!";
            BackgroundWorker BW = new BackgroundWorker();
            BW.DoWork += BW_DoWork;
            BW.RunWorkerCompleted += BW_RunWorkerCompleted;
            BW.RunWorkerAsync();
            
        }

        private void BW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                resultLabel.Content = "Canceled!";
            }
            else if (e.Error != null)
            {
                resultLabel.Content = "Error: " + e.Error.Message;
            }
            else
            {
                resultLabel.Content = "Done!";
                outcomeLabel.Content = Math.Round(Engine.Predict(new Shape(Engine.shape)),2) + " m";
            }
        }

        private void BW_DoWork(object sender, DoWorkEventArgs e)
        {
            Engine.population.Clear();
            for (int i = 0; i < 100; i++)
            {
                Engine.population.Add(new Entity(new Shape()));
            }
            Engine.IterateShapes();
            Engine.shape = new Shape(Engine.population[0].shape);
            //Node b = new Node(2, 0, 0);
            //Node a = new Node(0, 0, 0);
            //Node c = new Node(1, 4, 0);
            //List<Muscle> muscles = new List<Muscle>();
            //muscles.Add(new Muscle(a, b,1,0,0,0));
            //muscles.Add(new Muscle(b, c,1,0,0,0));
            //muscles.Add(new Muscle(c, a,1,0,0,0));
            //Engine.shape = new Shape(muscles);
            canvas.Dispatcher.BeginInvoke(DispatcherPriority.Normal,new Action( () =>
            {
                canvas.Children.Add(Engine.Frame());
            }));


        }

        private void InitAll()
        {

            canvas.Children.Add(Engine.InitializeBackground(canvas.Width, canvas.Height));
            renderTimer = new DispatcherTimer();
            renderTimer.Interval = new TimeSpan(Engine.interval);
            renderTimer.Tick += Timer_Tick;

        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            if (Engine.seconds * Engine.tickrate <= Engine.tick)
            {
                renderTimer.Stop();
            }
            else
            {
                BackgroundWorker aux = new BackgroundWorker();
                aux.DoWork += Aux_DoWork;
                aux.RunWorkerAsync();
            }
        }

        private void Aux_DoWork(object sender, DoWorkEventArgs e)
        {
            Tick();
        }

        private void Tick()
        {
            Engine.tick++;
            Engine.Physics(Engine.shape);
            canvas.Dispatcher.BeginInvoke(DispatcherPriority.Normal,new Action( () =>
            {
                canvas.Children.Clear();
                canvas.Children.Add(Engine.Frame());
            }));
            listbox.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                listbox.Items.Clear();
                for (int i = 0; i < Engine.shape.nodes.Count; i++)
                {
                    listbox.Items.Add(string.Format("P{0} -> {1}i+ {2}j", i, Math.Round(Engine.shape.nodes[i].GetSpeed().X), Math.Round(Engine.shape.nodes[i].GetSpeed().Y - Engine.shape.nodes[i].gCount * Engine.g/Engine.tickrate)));
                }
            }));
            
        }

        private void testButton_Click_1(object sender, RoutedEventArgs e)
        {
            renderTimer.Start();
        }

        private void introduce_speed_Click(object sender, RoutedEventArgs e)
        {
            Node a = Engine.shape.nodes[1];
            Node b = Engine.shape.nodes[0];
            a.SetSpeed(b.GetSpeed() + new Vector(0,0));
            b.SetSpeed(a.GetSpeed() + new Vector(100, 100));
        }

        private void step_Click(object sender, RoutedEventArgs e)
        {
            renderTimer.Stop();
            Tick();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            renderTimer.Stop();
            Test();
        }

        private void listbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


            int index = listbox.SelectedIndex;
            if (index == -1)
            {

            }
            else
                Engine.selected = Engine.shape.nodes[index];


        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            outcomeLabel.Content = Engine.Predict(new Shape(Engine.shape));
        }
    }
}
