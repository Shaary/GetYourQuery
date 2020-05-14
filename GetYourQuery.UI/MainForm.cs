using System;
using System.Windows.Forms;

namespace GetYourQuery.UI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        public void btnFind_Click(object sender, EventArgs e)
        {

        }

        private void LoadComboBox(ComboBox comboBox, string valueName)
        {
            comboBox.DataSource = null;
            comboBox.Items.Clear();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        private void ReloadDataFields()
        {
            try
            {
                LoadComboBox(spTypeBox, "");
                LoadComboBox(schemaBox, "");
                
            }
            catch (Exception e)
            {
                MessageBox.Show("Problem connecting to database\n" + e.ToString());
            }
        }
    }
}
