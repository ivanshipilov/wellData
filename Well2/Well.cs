using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Drawing;
using System.Windows.Forms;

namespace well
{
    public class Well
    {
        public static int count = 0;
        private string wellName;
        private string wellPath;
        private decimal wellDepth;
        public string WellName { get { return wellName; } }
        public string WellPath { get { return wellPath; } }
        public decimal WellDepth { get { return wellDepth; } }
        List<string> methodsList = new List<string>();
        Dictionary<string, List<decimal>> dataDict = new Dictionary<string, List<decimal>>();
        

        //constructor
        public Well(string wellName, string wellPath)
        {            
            this.wellName = wellName;
            this.wellPath = wellPath;            
            GetWellMethodsFromFile(wellPath);
            GetWellDataFromFile(wellPath);
            EstimateWellDepth();
            Well.count++;
        }       

        //parse well methods
        public void GetWellMethodsFromFile(string wellPath)
        {
            StreamReader reader = new StreamReader(wellPath);
            int counter = 0;
            int numLine = 1;
            int methodsCount = 0;
            string line;
            string curve = "~C";
            string param = "~P";
            string hash = "#";
            bool curveFinded = false;
            //List<string> methods = new List<string>();
            while ((line = reader.ReadLine()) != null)
            {
                counter++;
                if (line.StartsWith(curve))
                {
                    curveFinded = true;
                    numLine = counter;
                    continue;
                }
                if (curveFinded && line.StartsWith(hash) && ((counter - numLine) == 1))
                {
                    numLine++; continue;
                }
                else
                {
                    if (curveFinded && !line.StartsWith(param))
                    {
                        string[] word = line.Split(new string[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
                        this.methodsList.Add(word[methodsCount]);
                        methodsCount += methodsCount;
                    }
                    else
                    {
                        if (line.StartsWith(param))
                        {
                            break;
                        }
                        continue;
                    }
                }
            }
        }

        //parse well data
        public Dictionary<string, List<decimal>> GetWellDataFromFile(string wellPath)
        {
            int counter = 0;
            string line;
            int numLine = 1;
            int countValues = 0;
            string ascii = "~A";
            string hash = "#";
            bool asciiFinded = false;
            List<string> methodsList = new List<string>();

            //запись в лист с методами методов скважины из функции WellMethods
            foreach (string method in this.methodsList)
            {
                methodsList.Add(method);
            }

            //StreamReader reader1 = null; 
            //try
            //{
            //    reader1 = new StreamReader(wellPath);
            //    string s = wellName;
            //    WellMethods(wellPath);

            //    throw new Exception();
            //}
            //catch(Exception ex)
            //{
            //    System.Windows.Forms.MessageBox.Show(ex.ToString());
            //}
            //finally
            //{
            //    System.Windows.Forms.MessageBox.Show("Test 123");
            //    if(reader1 != null)
            //    {
            //        reader1.Close();
            //    }
            //}

            using (StreamReader reader = new StreamReader(wellPath))
            {//fix
                while ((line = reader.ReadLine()) != null)
                {
                    counter++;
                    if (line.StartsWith(ascii))
                    {
                        asciiFinded = true;
                        numLine = counter;
                        continue;
                    }
                    if (asciiFinded && line.StartsWith(hash) && ((counter - numLine) == 1))
                    {
                        numLine++;
                        continue;
                    }
                    else
                    {
                        if (asciiFinded)
                        {
                            string[] word = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            int methodNum = 0;
                            foreach (string wrd in word)
                            {
                                decimal val;
                                decimal.TryParse(wrd, NumberStyles.Any, CultureInfo.InvariantCulture, out val);

                                if (countValues == 0)
                                {
                                    dataDict.Add(methodsList[methodNum], new List<decimal>());
                                    dataDict[methodsList[methodNum]].Add(val);
                                }
                                else
                                {
                                    dataDict[methodsList[methodNum]].Add(val);
                                }
                                methodNum++;
                            }
                            countValues++;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
            return dataDict;
        }

        //show well methods
        public List<string> WellMethods()
        {
            return this.methodsList;
        }

        //show well data
        public Dictionary<string, List<decimal>> WellData()
        {
            return this.dataDict;
        }//убрал получение аргумента string wellPath

        //estimate wellDepth
        private void EstimateWellDepth()
        {
            string depthDataKey = dataDict.First().Key;
            this.wellDepth = dataDict[depthDataKey].Last();
        }
    }
}
