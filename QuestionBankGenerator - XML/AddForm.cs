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
    public partial class AddForm : Form
    {
        // Importing stuff from main form
        string xmlType = MainForm.xmlType;
        string MCQ = MainForm.MCQ;
        string RAPID_FIRE = MainForm.RAPID_FIRE;

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

        public void addQuestion()
        {
            // Gets and loads the static var XML file from Form1
            string xmlFile = MainForm.xmlFile;

            XmlDocument xmlDoc = new XmlDocument();

            // Loads the XML file in the memory for editting
            xmlDoc.Load(xmlFile);
            
            // Gets the root node of the XML document (expected to be <questions>)
            XmlElement rootNode = xmlDoc.DocumentElement;

            // If the xmlType is MCQ
            if (xmlType == MCQ)
            {
                // Collection of all the question nodes (will be filled later)
                List<string> questionTags = new List<string>();

                // Fills the questionNodes list
                foreach (XmlElement presetQuestionNode in rootNode)
                {
                    questionTags.Add(presetQuestionNode.Name);
                }

                // A little non-intuitive question number management part
                List<int> intArray = new List<int>();

                foreach (string question in questionTags)
                {
                    intArray.Add(int.Parse(question.Replace("question_", "")));
                }

                intArray.Sort();
                List<int> missingNumbers = new List<int>();
                int questionNumber;

                for (int i = 0; i < intArray.Count; i++)
                {
                    if (intArray[i] != i)
                        missingNumbers.Add(i);
                }

                if (missingNumbers.Count != 0)
                    questionNumber = missingNumbers[0];
                else if (questionTags.Count != 1)
                    questionNumber = questionTags.Count;
                else
                    questionNumber = 1;

                string elementName = "question_" + questionNumber.ToString();

                // Creating XML empty nodes to hold the respective values
                XmlNode questionNode = xmlDoc.CreateElement(elementName);
                XmlNode title = xmlDoc.CreateElement("title");
                XmlNode answer1 = xmlDoc.CreateElement("answer1");
                XmlNode answer2 = xmlDoc.CreateElement("answer2");
                XmlNode answer3 = xmlDoc.CreateElement("answer3");
                XmlNode answer4 = xmlDoc.CreateElement("answer4");
                XmlNode correctAns = xmlDoc.CreateElement("answerCorrect");

                // Appending values to XML nodes
                title.InnerText = question;
                answer1.InnerText = answers[0];
                answer2.InnerText = answers[1];
                answer3.InnerText = answers[2];
                answer4.InnerText = answers[3];
                correctAns.InnerText = correctAnswer;

                // Appending XML nodes as child nodes to respective parents
                rootNode.AppendChild(questionNode);
                questionNode.AppendChild(title);
                questionNode.AppendChild(answer1);
                questionNode.AppendChild(answer2);
                questionNode.AppendChild(answer3);
                questionNode.AppendChild(answer4);
                questionNode.AppendChild(correctAns);

                // Success message
                MessageBox.Show("Your question has been added as question number " +
                    questionNumber.ToString());
            }
            // If the xmlType is rapidfire
            else
            {
                XmlNode questionNode = xmlDoc.CreateElement("question");
                questionNode.InnerText = question;
                rootNode.AppendChild(questionNode);
            }

            // Save the file to the user inputted file
            xmlDoc.Save(xmlFile);
            Close();
        }

        public AddForm()
        {
            InitializeComponent();
        }

        private void AddForm_Load(object sender, EventArgs e)
        {
            groupBox2.Hide();
            if (xmlType == RAPID_FIRE)
            {
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                textBox4.Enabled = false;
                textBox5.Enabled = false;
            }

        }

        // Continue button
        private void button2_Click(object sender, EventArgs e)
        {
            // If the xmlType is MCQ
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
                else
                {
                    MessageBox.Show(
                        "You have left some text feild(s) empty", "Seriously?");
                }
            }
            // If the xmlType is rapidfire
            else
            {
                question = textBox1.Text.ToString();
                addQuestion();
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
            
            if(correctAnswer != null)
                addQuestion();
        }
    }
}
