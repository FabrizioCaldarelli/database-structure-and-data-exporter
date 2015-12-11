using DatabaseStructureAndData.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DatabaseStructureAndDataExporter
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DatabaseParser databaseParser;
        List<DatabaseStructureAndData.Classes.Table> tables = null;

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.txtPathExportJson.Text = (String.Format(@"{0}\json", Environment.GetFolderPath(Environment.SpecialFolder.Desktop)));
            this.txtPathExportCSharpModel.Text = (String.Format(@"{0}\csharpModels", Environment.GetFolderPath(Environment.SpecialFolder.Desktop)));
        }

        public void parseStructureAndData(String host, String database)
        {
            databaseParser = new DatabaseParser(host, database);
            databaseParser.connect();
            tables = databaseParser.listTablesWithStructureAndData();
        }

        private void btnParseStructureAndData_Click(object sender, RoutedEventArgs e)
        {
            String host = txtHost.Text;
            String database = txtDatabase.Text;

            parseStructureAndData(host, database);

            MessageBox.Show("Parsed Structure and Data!");
        }

        private void btnExportJson_Click(object sender, RoutedEventArgs e)
        {
            if (tables == null)
            {
                MessageBox.Show("Launch Parse Structure and Data first!");
                return;
            }

            foreach (DatabaseStructureAndData.Classes.Table t in tables)
            {
                String json = t.exportDataToJson();

                String pathBase = txtPathExportJson.Text;
                System.IO.Directory.CreateDirectory(pathBase);
                String pathFile = String.Format(@"{0}\{1}.json", pathBase, t.Name.Replace(" ", ""));

                System.IO.File.WriteAllText(pathFile, json);
            }

            MessageBox.Show("JSON Exported!");

        }

        private void btnExportCSharpModel_Click(object sender, RoutedEventArgs e)
        {
            String modelNamespace = txtNamespaceSrc.Text;

            if (tables == null)
            {
                MessageBox.Show("Launch Parse Structure and Data first!");
                return;
            }

            String pathBase = txtPathExportCSharpModel.Text;
            System.IO.Directory.CreateDirectory(pathBase);
            String pathFile = String.Format(@"{0}\BaseModel.cs", pathBase);

            String strBase = DatabaseStructureAndData.Classes.Table.exportStructurCSharpBaseModel(modelNamespace);

            System.IO.File.WriteAllText(pathFile, strBase);



            foreach (DatabaseStructureAndData.Classes.Table t in tables)
            {
                pathFile = String.Format(@"{0}\{1}.cs", pathBase, t.Name.Replace(" ", ""));

                List<String> righeMember = new List<string>();
                List<String> righeProperty = new List<string>();
                foreach (KeyValuePair<String, Column> kvp in t.Columns)
                {
                    righeMember.Add(kvp.Value.exportToCSharpMember());
                    righeProperty.Add(kvp.Value.exportToCSharpProperty());
                }

                String strRigheMember = String.Join("\n\t\t\t", righeMember);
                String strRigheProperty = String.Join("\n\t\t\t", righeProperty);

                String cf = t.exportStructureToCSharpModel(modelNamespace);

                System.IO.File.WriteAllText(pathFile, cf);

            }

            MessageBox.Show("C# Models Exported!");
        }
    }

}
