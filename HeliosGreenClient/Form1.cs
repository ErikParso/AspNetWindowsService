using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace HeliosGreenClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Text = Assembly.GetEntryAssembly().Location;
            label1.Text = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText("versionInfo.json")).version;
            label3.Text = Environment.CommandLine;
        }

        private async void SimpleButton1_Click(object sender, System.EventArgs e)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress =new System.Uri( @"http://localhost:5000");
                var installDir = new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName;
                var content = JsonConvert.SerializeObject(new { InstallDir = installDir, Extension = textBox1.Text });
                HttpContent contentPost = new StringContent(content, Encoding.UTF8, "application/json");
                await httpClient.PostAsync("api/association", contentPost);
                Close();
            }
        }
    }
}
