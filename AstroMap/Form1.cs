using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AstroMap
{
    public partial class Form1 : Form
    {
        private MapApi Map;
        private DataProcessor<MeteorBase> Processor;

        public Form1()
        {
            InitializeComponent();
            Map = new MapApi();
            Processor = null;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            var wm = await Map.GetWorldMap();
            mapPictureBox.Image = wm;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string pathToCsv = openFileDialog1.FileName;
                NasaCsv.NasaCsvParser parser = new NasaCsv.NasaCsvParser(pathToCsv);
                var _proocessor = new DataProcessor<NasaCsv.Meteor>(parser);
                Processor = _proocessor;

                labelDatasetName.Text = openFileDialog1.SafeFileName;
            }
            else
            {
                MessageBox.Show("Cant Open File Dialog");
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (Processor != null)
            {
                var wm = await Map.GetHeatMap(Processor.GetMeteors());
                mapPictureBox.Image = wm;
            }
            else
            {
                MessageBox.Show("Dataset not Picked");
            }
        }
    }
}
