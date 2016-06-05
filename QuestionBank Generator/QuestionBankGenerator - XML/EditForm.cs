using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace QuestionBankGenerator___XML
{
    public partial class EditForm : Form
    {
        // Getting the stuff we need
        string xmlFile = MainForm.xmlFile;
        string xmlType = MainForm.xmlType;
        string MCQ = MainForm.MCQ;
        string RAPID_FIRE = MainForm.RAPID_FIRE;
        XmlNode editQuestion = MainForm.selectedQuestion;

        // These variables store the inputs
        string question, correctAnswer;
        List<string> answers = new List<string>();

        public void switchGroupBox()
        {
            // Moving on to the next phase
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            textBox3.Enabled = false;
            textBox4.Enabled = false;
            textBox5.Enabled = false;
            groupBox2.Show();

            textBox6.Text = question;
            radioButton1.Text = answers[0];
            radioButton2.Text = answers[1];
            radioButton3.Text = answers[2];
            radioButton4.Text = answers[3];
        }

        // Method to update the question XML
        public void updateQuestion()
        {
            XmlDocument xmlDoc = new XmlDocument();
            // Loads the XML file to the memory for editing
            XmlTextReader reader = new XmlTextReader(xmlFile);
            xmlDoc.Load(reader);

            // Renders a list of all the childNodes in the question
            // the user is editing

            // If the xmlType is MCQ
            if (xmlType == MCQ)
            {
                XmlNode question = editQuestion.ParentNode.ParentNode;

                xmlDoc.SelectSingleNode(question.Name.ToString() +
                    "/" + editQuestion.ParentNode.Name.ToString() +
                    "/title").InnerText = textBox1.Text;

                xmlDoc.SelectSingleNode(question.Name.ToString() +
                    "/" + editQuestion.ParentNode.Name.ToString() +
                    "/answer1").InnerText = textBox2.Text;

                xmlDoc.SelectSingleNode(question.Name.ToString() +
                    "/" + editQuestion.ParentNode.Name.ToString() +
                    "/answer2").InnerText = textBox2.Text;

                xmlDoc.SelectSingleNode(question.Name.ToString() +
                    "/" + editQuestion.ParentNode.Name.ToString() +
                    "/answer3").InnerText = textBox3.Text;

                xmlDoc.SelectSingleNode(question.Name.ToString() +
                    "/" + editQuestion.ParentNode.Name.ToString() +
                    "/answer4").InnerText = textBox4.Text;

                xmlDoc.SelectSingleNode(question.Name.ToString() +
                    "/" + editQuestion.ParentNode.Name.ToString() +
                    "/answerCorrect").InnerText = textBox5.Text;

                
            }
            // If the xmlType is rapidfire
            else
            {
                XmlNode rootNode = xmlDoc.DocumentElement;
                XmlNodeList childNodes = rootNode.ChildNodes;
                XmlNode editNode, newNode;

                List<XmlNode> container = new List<XmlNode>();

                foreach (XmlNode questionNode in childNodes)
                {
                    if (questionNode.InnerText == editQuestion.InnerText)
                        container.Add(questionNode);
                }

                editNode = container[0];
                newNode = editNode;
                newNode.InnerText = textBox1.Text;

                rootNode.ReplaceChild(newNode, editNode);
            }
            reader.Close();
            xmlDoc.Save(xmlFile);

            MessageBox.Show("The question has been updated!");
            Close();
        }

        public EditForm()
        {
            InitializeComponent();
        }

        private void EditForm_Load(object sender, EventArgs e)
        {
            // Hide the second phase
            groupBox2.Hide();

            // Render the values from XML file to respective text boxes
            if (xmlType == MCQ)
            {
                XmlNodeList childNodes = editQuestion.ParentNode.ChildNodes;
                textBox1.Text = childNodes[0].InnerText;
                textBox2.Text = childNodes[1].InnerText;
                textBox3.Text = childNodes[2].InnerText;
                textBox4.Text = childNodes[3].InnerText;
                textBox5.Text = childNodes[4].InnerText;
            }
            else
            {
                textBox1.Text = editQuestion.InnerText;
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                textBox4.Enabled = false;
                textBox5.Enabled = false;
            }
        }

        // Second continue button
        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
                correctAnswer = "1";
            else if (radioButton2.Checked)
                correctAnswer = "2";
            else if (radioButton3.Checked)
                correctAnswer = "3";
            else if (radioButton4.Checked)
                correctAnswer = "4";
            else
                MessageBox.Show("You haven't selected an option.", "Seriously?");

            if (correctAnswer != null)
                updateQuestion();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (xmlType == MCQ)
            {
                // Checks if any of the text fields has been left empty
                if (textBox1.Text != "" && textBox2.Text != "" &&
                    textBox3.Text != "" && textBox4.Text != "" &&
                    textBox5.Text != "")
                {
                    // question = the text entered by the user in textBox1
                    question = textBox1.Text.ToString();

                    // Array of strings containing all the input answers
                    string[] inputs = {
                textBox2.Text.ToString(),
                textBox3.Text.ToString(),
                textBox4.Text.ToString(),
                textBox5.Text.ToString(),
                };

                    // Appends each answer to the answers collection
                    foreach (string answer in inputs)
                    {
                        answers.Add(answer);
                    }
                    // Continues to the next phase
                    switchGroupBox();
                }
            }
            else
            {
                question = textBox1.Text.ToString();
                updateQuestion();
            }
        }
    }
}
