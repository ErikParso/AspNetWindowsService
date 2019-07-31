using Newtonsoft.Json;
using System.IO;
using System.Windows.Forms;

namespace HeliosGreenClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            label1.Text = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText("versionInfo.json")).version;
        }
    }
}
