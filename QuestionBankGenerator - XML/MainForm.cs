using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using System.Diagnostics;

namespace QuestionBankGenerator___XML
{
    public partial class MainForm : Form
    {
        // Variable to store the user defined XML file path (xmlFile)
        public static string xmlFile;
        // Variables to store the path to new XML files
        public static string mcqPath = 
            Environment.ExpandEnvironmentVariables(
                @"%USERPROFILE%\Documents\questions.xml");
        public static string rpfPath =
            Environment.ExpandEnvironmentVariables(
                @"%USERPROFILE%\Documents\rapidfire.xml");

        // This will store the XML file type (MCQ or Rapidfire)
        public static string xmlType;

        // Constants to avoid typos
        public static string RAPID_FIRE = "RapidFire";
        public static string MCQ = "MCQ";
        
        // This dictionary will have the question (string) as the key
        // and the XmlNode (Name = title) as the value, may look weird
        // but this will be useful
        Dictionary<string, XmlNode> questionsDict =
            new Dictionary<string, XmlNode>();

        // Variable for editing the selected question
        public static XmlNode selectedQuestion;

        // Button controls
        public void enableButtons(string name)
        {
            if (name == "default")
            {
                button6.Enabled = true; // Edit
            }
            else if (name == "remove")
                button4.Enabled = true; // Remove
        }

        public void disableButtons(string name)
        {
            if (name == "default") 
                {
                button6.Enabled = false; // Edit
                }
            else if (name == "remove")
                button4.Enabled = false; // Remove
        }

        // Control button behavior
        public void checkButtons()
        {
            // List box edit button behavior
            if (listBox1.SelectedIndices.Count > 1 ||
                listBox1.SelectedIndices.Count == 0)
                disableButtons("default");
            if (listBox1.SelectedIndices.Count == 0)
                disableButtons("remove");
            if (listBox1.SelectedIndices.Count == 1)
                enableButtons("default");
            if (listBox1.SelectedIndices.Count >= 1)
                enableButtons("remove");
        }

        // A function to load the xmlFile into memory since we will
        // be doing it frequently in the entire code
        public XmlDocument loadXml()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlFile);
            return xmlDoc;
        }

        public void getXMLType()
        {
            XmlDocument xmlDoc = loadXml();

            // Gets the root node
            XmlNode rootNode = xmlDoc.DocumentElement;

            if (rootNode.Name == "questions")
                xmlType = MCQ;
            else
                xmlType = RAPID_FIRE;
        }

        public void getQuestions()
        {
            XmlDocument xmlDoc = loadXml();
            XmlElement rootNode = xmlDoc.DocumentElement;

            // Fills the list of questions by iterating over the nodes 
            // in the XML file

            // If XML file is for MCQ
            if (xmlType == MCQ)
            {
                foreach (XmlNode questionNode in rootNode)
                {
                    if (questionNode.Name != "question_0")
                    {
                        foreach (XmlNode itemNode in questionNode)
                        {
                            if (itemNode.Name == "title")
                                questionsDict[itemNode.InnerText] = itemNode;
                        }
                    }
                }
            }
            // If XML file is for RapidFire
            else
            {
                foreach (XmlNode questionNode in rootNode)
                {
                    if (questionNode.Name == "question")
                        questionsDict[questionNode.InnerText] = questionNode;
                }
            }
        }

        // Syncs the questions list box
        public void syncListBox()
        {
            // Clear the questions dictionary and the listBox entries
            questionsDict.Clear();
            listBox1.Items.Clear();

            // Calling method to construct the questionsDict dictionary
            getQuestions();

            // Adding questions to the listbox from questionsDict keys
            foreach (string question in
                new List<string>(questionsDict.Keys))
            {
                listBox1.Items.Add(question);
            }
            checkButtons();
        }

        // Switches to the second phase - Editing the XML file
        public void switchToSecondBox()
        {
            getXMLType();
            button1.Enabled = false;
            button2.Enabled = false;
            groupBox2.Show();
            syncListBox();
        }

        // Method to create a new XML file in My Documents
        public void createNewFile(string xmlFormat)
        {
            XmlDocument doc = new XmlDocument();
            // Declares the XML stuff
            XmlNode declaration = doc.CreateXmlDeclaration(
                "1.0", "utf-8", "");

            if (xmlFormat == MCQ)
            {
                XmlNode rootNode = doc.CreateElement("questions");
                XmlNode questionZero = doc.CreateElement("question_0");
                XmlNode title = doc.CreateElement("title");
                XmlNode answer1 = doc.CreateElement("answer1");
                XmlNode answer2 = doc.CreateElement("answer2");
                XmlNode answer3 = doc.CreateElement("answer3");
                XmlNode answer4 = doc.CreateElement("answer4");
                XmlNode correctAnswer = doc.CreateElement("answerCorrect");

                title.InnerText = " ";
                answer1.InnerText = " ";
                answer2.InnerText = " ";
                answer3.InnerText = " ";
                answer4.InnerText = " ";
                correctAnswer.InnerText = "0";

                doc.AppendChild(declaration);
                doc.AppendChild(rootNode);
                rootNode.AppendChild(questionZero);
                questionZero.AppendChild(title);
                questionZero.AppendChild(answer1);
                questionZero.AppendChild(answer2);
                questionZero.AppendChild(answer3);
                questionZero.AppendChild(answer4);
                questionZero.AppendChild(correctAnswer);
                doc.Save(mcqPath);

                MessageBox.Show("An XML file named \"questions.xml\" has been" +
                    "created in your \"My Documents\" workspace", "Success");

                xmlFile = mcqPath;
            }
            else
            {
                XmlNode rootNode = doc.CreateElement("rapidfire");
                XmlNode questionZero = doc.CreateElement("question_0");

                questionZero.InnerText = "Blank Question ";

                doc.AppendChild(rootNode);
                rootNode.AppendChild(questionZero);
                doc.Save(rpfPath);

                MessageBox.Show("An XML file named \"rapidfire.xml\" has been " +
                    "created in your \"My Documents\" workspace", "Sucess");

                xmlFile = rpfPath;
            }
            switchToSecondBox();
        }

        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Hide phase two while we are in phase 1
            groupBox2.Hide();

            checkButtons();
        }

        // Open button
        private void button1_Click(object sender, EventArgs e)
        {
            // Start an Open File Dialog
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = "Choose The Question Bank XML File";
            openFile.InitialDirectory = @"%USERPROFILE%\Documents";
            openFile.Filter = "XML files (*.xml)|*.xml";
            openFile.FilterIndex = 2;
            openFile.RestoreDirectory = true;
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                xmlFile = openFile.FileName;
            }

            // If the user pressed the cancel button stop further execution
            if (xmlFile == null)
                return;

            XmlDocument xmlDoc = new XmlDocument();

            try
            {
                xmlDoc = loadXml();
            }
            // If the XML file is invalid
            catch (XmlException)
            {
                MessageBox.Show("The file you selected is an invalid XML file", "Seriously?");
                return;
            }

            // Root node of the XML file
            XmlNode rootNode = xmlDoc.DocumentElement;

            // Check if the format of the XML file is correct
            // (in accordance to our custom format)
            if (xmlFile != null && 
                (rootNode.Name == "questions" || rootNode.Name == "rapidfire"))
                switchToSecondBox();
            else if (rootNode.Name != "questions" || 
                rootNode.Name != "rapidfire")
            {
                MessageBox.Show("The file you tried to load is " +
                    "not in the expected question file format", "Seriously?");
                return;
            }
        }

        // New button
        private void button2_Click(object sender, EventArgs e)
        {
            contextMenuStrip1.Show(MousePosition);
        }

        // New >> MCQ Format
        private void mCQFormatToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (File.Exists(mcqPath))
            {
                DialogResult warning = MessageBox.Show(
                    "A file named questions.xml already exists " +
                    "in \"MyDocument\", do you want to continue?",
                    "WARNING", MessageBoxButtons.OKCancel);

                if (warning == DialogResult.OK)
                    createNewFile(MCQ);
            }
            else
                createNewFile(MCQ);
        }

        // New >> Rapidfire Format
        private void rapidfireFormatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(rpfPath))
            {
                DialogResult warning = MessageBox.Show(
                    "A file named questions.xml already exists " +
                    "in \"MyDocument\", do you want to continue?",
                    "WARNING", MessageBoxButtons.OKCancel);

                if (warning == DialogResult.OK)
                    createNewFile(RAPID_FIRE);
            }
            else
                createNewFile(RAPID_FIRE);
        }

        // Questions ListBox
        private void listBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            checkButtons();
        }

        // Get Raw
        private void button5_Click(object sender, EventArgs e)
        {
            Process.Start("notepad.exe", xmlFile);
        }

        // Add button
        private void button3_Click(object sender, EventArgs e)
        {
            AddForm addForm = new AddForm();
            addForm.ShowDialog();
            syncListBox();
        }

        // Remove button
        private void button4_Click(object sender, EventArgs e)
        {
            // List of XmlNodes to be removed
            List<XmlNode> removeItems = new List<XmlNode>();

            // Iterates over the list of selected items in the listBox
            // and appends the XmlNode (obtained from questionsDict) to
            // the list of XmlNodes to be removed
            foreach (int index in listBox1.SelectedIndices)
            {
                removeItems.Add(
                    questionsDict[listBox1.Items[index].ToString()]);
            }

            XmlDocument xmlDoc = loadXml();
            XmlNode rootNode = xmlDoc.DocumentElement;

            // Finally we iterate over the list of XmlNodes to be removed
            foreach (XmlNode node in removeItems)
            {
                // Gets a list of all the nodes with the name of the node
                // to be removed
                XmlNodeList toBeRemoved =
                    xmlDoc.GetElementsByTagName(node.ParentNode.Name);

                rootNode.RemoveChild(toBeRemoved[0]);
                listBox1.Items.Remove(node.InnerText);
            }
            xmlDoc.Save(xmlFile);
        }

        // Edit button
        private void button6_Click(object sender, EventArgs e)
        {
            selectedQuestion = questionsDict
                [listBox1.Items[listBox1.SelectedIndex].ToString()];

            EditForm editForm = new EditForm();
            editForm.ShowDialog();
            syncListBox();
        }
    }
}
