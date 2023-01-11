using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace jobShopExample
{
    public partial class MainForm : Form
    {
        // data fileds
        int machines; // here we count the number of machines
        int jobs; // here we count the number of jobs

        public List<List<int>> J; // record gagain
        public List<List<int>> T;// record gagain
        List<List<int>> JobSequence;
        List<List<int>> TimeCosts;

        public List<Dictionary<int, int>> Data;
        public List<int> AllTimePoints;

        // here we show the new data
        public int[] prepareData; // here is n jobs, always have n data in the prepare data area, till to the end
        public int[] thenumberOfJob; // same as preare data, but it record the number
        public int[] machineTime; // record the time of machine each
        public int[] jobTime; // here record the time of job each

        public Random r = new Random();
        public Color[] c = { Color.Red, Color.Yellow, Color.Blue, Color.Green, Color.Cyan, Color.Orange, Color.Purple, Color.LightPink, Color.Gold, Color.Silver, Color.LightBlue, Color.LightGreen, Color.LightYellow, Color.LightPink, Color.LightSalmon, Color.DarkBlue, Color.DarkGreen, Color.DarkOrange, Color.DarkGoldenrod, Color.DarkKhaki };

        public MainForm()
        {
            InitializeComponent();
        }

        // here we open the data
        private void btnOpen_Click(object sender, EventArgs e)
        {
            // clear data 
            rtbShow.Clear();
            J = new List<List<int>>();
            T = new List<List<int>>();
            AllTimePoints = new List<int>();
            Data = new List<Dictionary<int, int>>();
            //JobSequence.Clear();
            //TimeCosts.Clear();
            //AllTimePoints.Clear();
            if (dlgOpen.ShowDialog() != DialogResult.OK) return;
            StreamReader sr = new StreamReader(dlgOpen.FileName);
            char[] seps = { ' ' };
            string[] JobsAndMachines = sr.ReadLine().Split(seps, StringSplitOptions.RemoveEmptyEntries);
            jobs = int.Parse(JobsAndMachines[0]);
            machines = int.Parse(JobsAndMachines[1]);
            rtbShow.AppendText($"Jobs Number: {jobs}, Machines Number: {machines}\n");
            rtbShow.AppendText("<<<Job Sequence Matrix>>>\n");
            for (int i = 0; i < jobs; i++)
            {
                string[] items = sr.ReadLine().Split(seps, StringSplitOptions.RemoveEmptyEntries);
                List<int> timecost = new List<int>();
                List<int> sequence = new List<int>();
                for (int j = 0; j < machines * 2; j++)
                {
                    if (j % 2 == 0)
                    {
                        sequence.Add(int.Parse(items[j]));
                    }
                    else
                    {
                        timecost.Add(int.Parse(items[j]));
                    }
                }
                //JobSequence.Add(sequence);
                //TimeCosts.Add(timecost);
                J.Add(sequence);
                T.Add(timecost);
            }
            for (int k = 0; k < jobs; k++)
            {
                rtbShow.AppendText($"job{k + 1}: ");
                for (int s = 0; s < machines; s++)
                {
                    rtbShow.AppendText($"{J[k][s] + 1} ");
                }
                rtbShow.AppendText("\n");

            }
            rtbShow.AppendText("<<<Time Cost Matrix>>>\n");
            for (int k = 0; k < jobs; k++)
            {
                rtbShow.AppendText($"job{k + 1}: ");
                for (int s = 0; s < machines; s++)
                {
                    rtbShow.AppendText($" {T[k][s]} ");
                }
                rtbShow.AppendText("\n");
            }


        }

        private void btnShortFirst_Click(object sender, EventArgs e)
        {
            // here we defined 2 matrix
            JobSequence = new List<List<int>>();
            TimeCosts = new List<List<int>>();
            for (int i = 0; i < jobs; i++)
            {
                List<int> timecost = new List<int>();
                List<int> sequence = new List<int>();
                for (int j = 0; j < machines; j++)
                {
                    sequence.Add(int.Parse(J[i][j].ToString()));
                    timecost.Add(int.Parse(T[i][j].ToString()));

                }
                JobSequence.Add(sequence);
                TimeCosts.Add(timecost);
            }

            // check how many data in our code
            machineTime = new int[machines];
            jobTime = new int[jobs];
            prepareData = new int[jobs];
            thenumberOfJob = new int[jobs];
            // here we get the data that we count
            for (int i = 0; i < jobs; i++)
            {
                // here set initial data
                jobTime[i] = 0;
                prepareData[i] = int.Parse(TimeCosts[i][0].ToString());
                thenumberOfJob[i] = int.Parse(JobSequence[i][0].ToString());
                JobSequence[i].RemoveAt(0);
                TimeCosts[i].RemoveAt(0);
            }
            for (int i = 0; i < machines; i++)
            {
                machineTime[i] = 0;
            }
            // clear data
            chtShortestFirst.Series.Clear();
            // here we new the series
            for (int i = 0; i < jobs; i++)
            {
                Series s = new Series();
                s.ChartType = SeriesChartType.RangeBar;
                s.Color = c[i];
                s.Name = $"Job{i + 1}";
                chtShortestFirst.Series.Add(s);
                chtShortestFirst.Series[i].SetCustomProperty("DrawSideBySide", "False");

            }
            for (int i = 0; i < jobs * machines; i++)
            {
                int Min = prepareData.Min();
                List<int> recordCost = new List<int>();
                List<int> recordNumber = new List<int>();
                List<int> recordj = new List<int>();
                for (int j = 0; j < jobs; j++)
                {
                    if (prepareData[j] == Min)
                    {
                        recordCost.Add(prepareData[j]);
                        recordNumber.Add(thenumberOfJob[j]); // record the machine number
                        recordj.Add(j); // record job number
                    }
                }
                int getrandom = r.Next(recordCost.Count);
                DataPoint dp;
                if (jobTime[recordj[getrandom]] >= machineTime[thenumberOfJob[recordj[getrandom]]])
                {
                    dp = new DataPoint();
                    dp.BorderColor = Color.DarkGray;
                    dp.Color = c[recordj[getrandom]];
                    dp.XValue = thenumberOfJob[recordj[getrandom]] + 1;
                    int end = jobTime[recordj[getrandom]] + prepareData[recordj[getrandom]];
                    dp.YValues = new double[2] { jobTime[recordj[getrandom]], end }; // here we draw the chart

                    // here we record all time points
                    AllTimePoints.Add(jobTime[recordj[getrandom]]);
                    AllTimePoints.Add(end);
                    AllTimePoints.Add(prepareData[recordj[getrandom]]);
                    // the jobtime, and the machine time need increate
                    jobTime[recordj[getrandom]] += prepareData[recordj[getrandom]]; //[recordj[getrandom]] record the number of select data
                    machineTime[thenumberOfJob[recordj[getrandom]]] = jobTime[recordj[getrandom]];
                    // here we draw the chart
                    chtShortestFirst.Series[recordj[getrandom]].Points.Add(dp);
                }
                else
                {
                    dp = new DataPoint();
                    dp.BorderColor = Color.DarkGray;
                    dp.Color = c[recordj[getrandom]];
                    dp.XValue = thenumberOfJob[recordj[getrandom]] + 1;
                    int end = machineTime[thenumberOfJob[recordj[getrandom]]] + prepareData[recordj[getrandom]];
                    dp.YValues = new double[2] { machineTime[thenumberOfJob[recordj[getrandom]]], end }; // here we draw the chart

                    // here we record all time points
                    AllTimePoints.Add(machineTime[thenumberOfJob[recordj[getrandom]]]);
                    AllTimePoints.Add(end);
                    //AllTimePoints.Add(prepareData[recordj[getrandom]]);
                    // the jobtime, and the machine time need increate
                    machineTime[thenumberOfJob[recordj[getrandom]]] += prepareData[recordj[getrandom]];
                    jobTime[recordj[getrandom]] = machineTime[thenumberOfJob[recordj[getrandom]]]; //[recordj[getrandom]] record the number of select data
                    // here we draw the chart
                    chtShortestFirst.Series[recordj[getrandom]].Points.Add(dp);
                }

                // here we remove the old data and add new data
                prepareData[recordj[getrandom]] = TimeCosts[recordj[getrandom]][0];
                thenumberOfJob[recordj[getrandom]] = JobSequence[recordj[getrandom]][0];
                TimeCosts[recordj[getrandom]].RemoveAt(0);
                JobSequence[recordj[getrandom]].RemoveAt(0);
                // check wether or not the data is empty
                if (JobSequence[recordj[getrandom]].Count == 0 && prepareData[recordj[getrandom]] != 100)
                {
                    TimeCosts[recordj[getrandom]].Add(100);
                    JobSequence[recordj[getrandom]].Add(11);
                }
            }
            rtbShortestFirst.Clear();
            rtbShortestFirst.AppendText($"the Maximum Span: {AllTimePoints.Max()}");
            AllTimePoints.Clear();
        }

        private void btnLongestFirst_Click(object sender, EventArgs e)
        {

            // clear data
            chtLongestFirst.Series.Clear();

            // here we defined 2 matrix
            JobSequence = new List<List<int>>();
            TimeCosts = new List<List<int>>();
            for (int i = 0; i < jobs; i++)
            {
                List<int> timecost = new List<int>();
                List<int> sequence = new List<int>();
                for (int j = 0; j < machines; j++)
                {
                    sequence.Add(int.Parse(J[i][j].ToString()));
                    timecost.Add(int.Parse(T[i][j].ToString()));

                }
                JobSequence.Add(sequence);
                TimeCosts.Add(timecost);
            }
            // check how many data in our code
            machineTime = new int[machines];
            jobTime = new int[jobs];
            prepareData = new int[jobs];
            thenumberOfJob = new int[jobs];
            // here we get the data that we count
            for (int i = 0; i < jobs; i++)
            {
                // here set initial data
                jobTime[i] = 0;
                prepareData[i] = int.Parse(TimeCosts[i][0].ToString());
                thenumberOfJob[i] = int.Parse(JobSequence[i][0].ToString());
                JobSequence[i].RemoveAt(0);
                TimeCosts[i].RemoveAt(0);
            }
            for (int i = 0; i < machines; i++)
            {
                machineTime[i] = 0;
            }

            // here we new the series
            for (int i = 0; i < jobs; i++)
            {
                Series s = new Series();
                s.ChartType = SeriesChartType.RangeBar;
                s.Color = c[i];
                s.Name = $"Job{i + 1}";
                chtLongestFirst.Series.Add(s);
                chtLongestFirst.Series[i].SetCustomProperty("DrawSideBySide", "False");

            }
            for (int i = 0; i < jobs * machines; i++)
            {
                int Max = prepareData.Max();
                List<int> recordCost = new List<int>();
                List<int> recordNumber = new List<int>();
                List<int> recordj = new List<int>();
                for (int j = 0; j < jobs; j++)
                {
                    if (prepareData[j] == Max)
                    {
                        recordCost.Add(prepareData[j]);
                        recordNumber.Add(thenumberOfJob[j]); // record the machine number
                        recordj.Add(j); // record job number
                    }
                }
                int getrandom = r.Next(recordCost.Count);
                DataPoint dp;
                if (jobTime[recordj[getrandom]] >= machineTime[thenumberOfJob[recordj[getrandom]]])
                {
                    dp = new DataPoint();
                    dp.BorderColor = Color.DarkGray;
                    dp.Color = c[recordj[getrandom]];
                    dp.XValue = thenumberOfJob[recordj[getrandom]] + 1;
                    int end = jobTime[recordj[getrandom]] + prepareData[recordj[getrandom]];
                    dp.YValues = new double[2] { jobTime[recordj[getrandom]], end }; // here we draw the chart

                    // here we record all time points
                    AllTimePoints.Add(jobTime[recordj[getrandom]]);
                    AllTimePoints.Add(end);

                    // the jobtime, and the machine time need increate
                    jobTime[recordj[getrandom]] += prepareData[recordj[getrandom]]; //[recordj[getrandom]] record the number of select data
                    machineTime[thenumberOfJob[recordj[getrandom]]] = jobTime[recordj[getrandom]];
                    // here we draw the chart
                    chtLongestFirst.Series[recordj[getrandom]].Points.Add(dp);
                }
                else
                {
                    dp = new DataPoint();
                    dp.BorderColor = Color.DarkGray;
                    dp.Color = c[recordj[getrandom]];
                    dp.XValue = thenumberOfJob[recordj[getrandom]] + 1;
                    int end = machineTime[thenumberOfJob[recordj[getrandom]]] + prepareData[recordj[getrandom]];
                    dp.YValues = new double[2] { machineTime[thenumberOfJob[recordj[getrandom]]], end }; // here we draw the chart

                    // here we record all time points
                    AllTimePoints.Add(machineTime[thenumberOfJob[recordj[getrandom]]]);
                    AllTimePoints.Add(end);

                    // the jobtime, and the machine time need increate
                    machineTime[thenumberOfJob[recordj[getrandom]]] += prepareData[recordj[getrandom]];
                    jobTime[recordj[getrandom]] = machineTime[thenumberOfJob[recordj[getrandom]]]; //[recordj[getrandom]] record the number of select data
                    // here we draw the chart
                    chtLongestFirst.Series[recordj[getrandom]].Points.Add(dp);
                }

                // here we remove the old data and add new data
                prepareData[recordj[getrandom]] = TimeCosts[recordj[getrandom]][0];
                thenumberOfJob[recordj[getrandom]] = JobSequence[recordj[getrandom]][0];
                TimeCosts[recordj[getrandom]].RemoveAt(0);
                JobSequence[recordj[getrandom]].RemoveAt(0);
                // check wether or not the data is empty
                if (JobSequence[recordj[getrandom]].Count == 0 && prepareData[recordj[getrandom]] != 0)
                {
                    TimeCosts[recordj[getrandom]].Add(0);
                    JobSequence[recordj[getrandom]].Add(11);
                }
            }
            rtbLongestFirst.Clear();
            rtbLongestFirst.AppendText($"the Maximum Span: {AllTimePoints.Max()}");
            AllTimePoints.Clear();
        }
        #region old code
        private void button1_Click(object sender, EventArgs e)
        {
            //public List<List<int>> sm = new List<List<int>>();
            //public List<List<int>> TimesPoint = new List<List<int>>();

            //// here is short first
            //for (int k = 0; k < jobs; k++)
            //{
            //    for (int i = 0; i < machines; i++)
            //    {
            //        for (int j = 0; j < i; j++)
            //        {
            //            if(TimeCosts[k][j] > TimeCosts[k][i])
            //            {
            //                int temp = TimeCosts[k][j];
            //                TimeCosts[k][j] = TimeCosts[k][i];
            //                TimeCosts[k][i] = temp;
            //                int temp_2 = JobSequence[k][j];
            //                JobSequence[k][j] = JobSequence[k][i];
            //                JobSequence[k][i] = temp_2;
            //            }
            //        }
            //    }
            //}

            ////here we add the data to the data, dictionary type
            //for (int i = 0; i < jobs; i++)
            //{
            //    Dictionary<int, int> s = new Dictionary<int, int>();
            //    for (int j = 0; j < machines; j++)
            //    {
            //        int key = JobSequence[i][j];
            //        int value = TimeCosts[i][j];
            //        s.Add(key, value);
            //    }
            //    Data.Add(s);
            //}
            //// here we count the time point
            //for (int i = 0; i < jobs; i++)
            //{
            //    List<int> time = new List<int>();
            //    for (int j = 0; j < machines; j++)
            //    {
            //        if(j == 0)
            //        {
            //            time.Add(TimeCosts[i][j]);
            //        }
            //        else
            //        {
            //            int add = 0;
            //            add = time[j - 1] + TimeCosts[i][j];
            //            time.Add(add);
            //        }
            //    }
            //    TimesPoint.Add(time);
            //}
            //for (int j = 0; j < jobs; j++)
            //{
            //    List<int> f = new List<int>();
            //    for (int i = 0; i < jobs; i++)
            //    {
            //        foreach (var item in Data[i])
            //        {
            //            if (item.Key == j)
            //            {
            //                f.Add(item.Value);
            //                break;
            //            }
            //        }
            //    }
            //    sm.Add(f);
            //}
            //// here we show the chart and the rich text
            //for (int i = 0; i < machines; i++)
            //{ 
            //    Series s = new Series();
            //    s.ChartType = SeriesChartType.StackedBar;
            //    s.Name = $"Machine{1 + i}";
            //    for (int j = 0; j < jobs; j++)
            //    {
            //        s.Points.AddXY($"Job {j + 1}", Data[j][i]);
            //    }
            //    chtShortestFirst.Series.Add(s); 
            //}



            //for (int k = 0; k < jobs; k++)
            //{
            //    for (int i = 0; i < machines; i++)
            //    {
            //        for (int j = 0; j < i; j++)
            //        {
            //            if (TimeCosts[k][j] < TimeCosts[k][i])
            //            {
            //                int temp = TimeCosts[k][j];
            //                TimeCosts[k][j] = TimeCosts[k][i];
            //                TimeCosts[k][i] = temp;
            //                JobSequence[k][j] = JobSequence[k][i];
            //                JobSequence[k][i] = temp;
            //            }
            //        }
            //    }
            //}


            //for (int i = 0; i < 10; i++)
            //{
            //    Series s = new Series();
            //    s.ChartType = SeriesChartType.RangeBar;
            //    s.Name = $"Series{i}";
            //}
            //chtShortestFirst.Series.Add("Series0");
            //chtShortestFirst.Series[0].Points.AddXY("Series0", 0, 10);
            //chtShortestFirst.Series[0].Points.AddXY("Series0", 10, 30);
            //chtShortestFirst.Series[0].Points.AddXY()
            //Series s_1 = new Series();
            //Series s_2 = new Series();
            //s_1.ChartType = SeriesChartType.RangeBar;
            //s_2.ChartType = SeriesChartType.RangeBar;
            //chtShortestFirst.Series.Add(s_1);
            //chtShortestFirst.Series.Add(s_2);
            //DataPoint dp = new DataPoint();
            //dp.XValue = 1;
            //dp.YValues = new double[] { 0, 10};
            //chtShortestFirst.Series[0].Points.Add(dp);
            //DataPoint d1 = new DataPoint();
            //d1.XValue = 0;
            //dp.YValues = new double[] { 10, 20 };
            //chtShortestFirst.Series[1].Points.Add(d1);
            //s_1.XValueMember = s_2.XValueMember;
            //chtShortestFirst.Series[0].Points.AddXY("10", 0, 10);
            //chtShortestFirst.Series[1].Points.AddXY("10", 10, 20);
        }
        #endregion

        private void btnRemainLength_Click(object sender, EventArgs e)
        {
            // here we defined 2 matrix
            JobSequence = new List<List<int>>();
            TimeCosts = new List<List<int>>();
            for (int i = 0; i < jobs; i++)
            {
                List<int> timecost = new List<int>();
                List<int> sequence = new List<int>();
                for (int j = 0; j < machines; j++)
                {
                    sequence.Add(int.Parse(J[i][j].ToString()));
                    timecost.Add(int.Parse(T[i][j].ToString()));

                }
                JobSequence.Add(sequence);
                TimeCosts.Add(timecost);
            }
            // check how many data in our code
            machineTime = new int[machines];
            jobTime = new int[jobs];
            prepareData = new int[jobs];
            thenumberOfJob = new int[jobs];
            // here we get the data that we count
            for (int i = 0; i < jobs; i++)
            {
                // here set initial data
                jobTime[i] = 0;
                prepareData[i] = int.Parse(TimeCosts[i][0].ToString());
                thenumberOfJob[i] = int.Parse(JobSequence[i][0].ToString());
                JobSequence[i].RemoveAt(0);
                TimeCosts[i].RemoveAt(0);
            }
            for (int i = 0; i < machines; i++)
            {
                machineTime[i] = 0;
            }

            // clear data
            chtRemainLength.Series.Clear();
            // here we new the series
            for (int i = 0; i < jobs; i++)
            {
                Series s = new Series();
                s.ChartType = SeriesChartType.RangeBar;
                s.Color = c[i];
                s.Name = $"Job{i + 1}";
                chtRemainLength.Series.Add(s);
                chtRemainLength.Series[i].SetCustomProperty("DrawSideBySide", "False");

            }

            for (int i = 0; i < jobs * machines; i++)
            {
                int Max = 0;
                for (int x = 0; x < jobs - 1; x++)
                {
                    if (x == 0)
                    {
                        Max = TimeCosts[x].Count;
                    }
                    if (Max < TimeCosts[x + 1].Count)
                    {
                        Max = TimeCosts[x + 1].Count;
                    }
                }
                List<int> recordCost = new List<int>();
                List<int> recordNumber = new List<int>();
                List<int> recordj = new List<int>();
                for (int j = 0; j < jobs; j++)
                {
                    if (TimeCosts[j].Count == Max)
                    {
                        recordCost.Add(prepareData[j]);
                        recordNumber.Add(thenumberOfJob[j]); // record the machine number
                        recordj.Add(j); // record job number
                    }
                }
                int getrandom = r.Next(recordCost.Count);
                DataPoint dp;
                if (jobTime[recordj[getrandom]] >= machineTime[thenumberOfJob[recordj[getrandom]]])
                {
                    dp = new DataPoint();
                    dp.BorderColor = Color.DarkGray;
                    dp.Color = c[recordj[getrandom]];
                    dp.XValue = thenumberOfJob[recordj[getrandom]] + 1;
                    int end = jobTime[recordj[getrandom]] + prepareData[recordj[getrandom]];
                    dp.YValues = new double[2] { jobTime[recordj[getrandom]], end }; // here we draw the chart

                    // here we record all time points
                    AllTimePoints.Add(jobTime[recordj[getrandom]]);
                    AllTimePoints.Add(end);
                    AllTimePoints.Add(prepareData[recordj[getrandom]]);
                    // the jobtime, and the machine time need increate
                    jobTime[recordj[getrandom]] += prepareData[recordj[getrandom]]; //[recordj[getrandom]] record the number of select data
                    machineTime[thenumberOfJob[recordj[getrandom]]] = jobTime[recordj[getrandom]];
                    // here we draw the chart
                    chtRemainLength.Series[recordj[getrandom]].Points.Add(dp);
                }
                else
                {
                    dp = new DataPoint();
                    dp.BorderColor = Color.DarkGray;
                    dp.Color = c[recordj[getrandom]];
                    dp.XValue = thenumberOfJob[recordj[getrandom]] + 1;
                    int end = machineTime[thenumberOfJob[recordj[getrandom]]] + prepareData[recordj[getrandom]];
                    dp.YValues = new double[2] { machineTime[thenumberOfJob[recordj[getrandom]]], end }; // here we draw the chart

                    // here we record all time points
                    AllTimePoints.Add(machineTime[thenumberOfJob[recordj[getrandom]]]);
                    AllTimePoints.Add(end);
                    //AllTimePoints.Add(prepareData[recordj[getrandom]]);
                    // the jobtime, and the machine time need increate
                    machineTime[thenumberOfJob[recordj[getrandom]]] += prepareData[recordj[getrandom]];
                    jobTime[recordj[getrandom]] = machineTime[thenumberOfJob[recordj[getrandom]]]; //[recordj[getrandom]] record the number of select data
                    // here we draw the chart
                    chtRemainLength.Series[recordj[getrandom]].Points.Add(dp);
                }

                // here we remove the old data and add new data
                prepareData[recordj[getrandom]] = TimeCosts[recordj[getrandom]][0];
                thenumberOfJob[recordj[getrandom]] = JobSequence[recordj[getrandom]][0];
                TimeCosts[recordj[getrandom]].RemoveAt(0);
                JobSequence[recordj[getrandom]].RemoveAt(0);
                // check wether or not the data is empty
                if (JobSequence[recordj[getrandom]].Count == 0 && prepareData[recordj[getrandom]] != 100)
                {
                    TimeCosts[recordj[getrandom]].Add(100);
                    JobSequence[recordj[getrandom]].Add(11);
                }
            }
            rtbRemainLength.Clear();
            rtbRemainLength.AppendText($"the Maximum Span: {AllTimePoints.Max()}");
            AllTimePoints.Clear();
        }

        private void btnRemainTime_Click(object sender, EventArgs e)
        {  // here we defined 2 matrix
            JobSequence = new List<List<int>>();
            TimeCosts = new List<List<int>>();
            for (int i = 0; i < jobs; i++)
            {
                List<int> timecost = new List<int>();
                List<int> sequence = new List<int>();
                for (int j = 0; j < machines; j++)
                {
                    sequence.Add(int.Parse(J[i][j].ToString()));
                    timecost.Add(int.Parse(T[i][j].ToString()));

                }
                JobSequence.Add(sequence);
                TimeCosts.Add(timecost);
            }
            // check how many data in our code
            machineTime = new int[machines];
            jobTime = new int[jobs];
            prepareData = new int[jobs];
            thenumberOfJob = new int[jobs];
            // here we get the data that we count
            for (int i = 0; i < jobs; i++)
            {
                // here set initial data
                jobTime[i] = 0;
                prepareData[i] = int.Parse(TimeCosts[i][0].ToString());
                thenumberOfJob[i] = int.Parse(JobSequence[i][0].ToString());
                JobSequence[i].RemoveAt(0);
                TimeCosts[i].RemoveAt(0);
            }
            for (int i = 0; i < machines; i++)
            {
                machineTime[i] = 0;
            }
            // clear data
            chtRemainTime.Series.Clear();
            // here we new the series
            for (int i = 0; i < jobs; i++)
            {
                Series s = new Series();
                s.ChartType = SeriesChartType.RangeBar;
                s.Color = c[i];
                s.Name = $"Job{i + 1}";
                chtRemainTime.Series.Add(s);
                chtRemainTime.Series[i].SetCustomProperty("DrawSideBySide", "False");
            }

            for (int i = 0; i < jobs * machines; i++)
            {
                int[] SumTotal = new int[jobs];
                int Max = 0;
                for (int s = 0; s < jobs; s++)
                {
                    SumTotal[s] = TimeCosts[s].Sum() + prepareData[s];
                    if (SumTotal[s] > Max)
                    {
                        Max = SumTotal[s];
                    }
                }
                List<int> recordCost = new List<int>();
                List<int> recordNumber = new List<int>();
                List<int> recordj = new List<int>();
                for (int j = 0; j < jobs; j++)
                {
                    if (SumTotal[j] == Max)
                    {
                        recordCost.Add(prepareData[j]);
                        recordNumber.Add(thenumberOfJob[j]); // record the machine number
                        recordj.Add(j); // record job number
                    }
                }
                int getrandom = r.Next(recordCost.Count);
                DataPoint dp;
                if (jobTime[recordj[getrandom]] >= machineTime[thenumberOfJob[recordj[getrandom]]])
                {
                    dp = new DataPoint();
                    dp.BorderColor = Color.DarkGray;
                    dp.Color = c[recordj[getrandom]];
                    dp.XValue = thenumberOfJob[recordj[getrandom]] + 1;
                    int end = jobTime[recordj[getrandom]] + prepareData[recordj[getrandom]];
                    dp.YValues = new double[2] { jobTime[recordj[getrandom]], end }; // here we draw the chart

                    // here we record all time points
                    AllTimePoints.Add(jobTime[recordj[getrandom]]);
                    AllTimePoints.Add(end);
                    AllTimePoints.Add(prepareData[recordj[getrandom]]);
                    // the jobtime, and the machine time need increate
                    jobTime[recordj[getrandom]] += prepareData[recordj[getrandom]]; //[recordj[getrandom]] record the number of select data
                    machineTime[thenumberOfJob[recordj[getrandom]]] = jobTime[recordj[getrandom]];
                    // here we draw the chart
                    chtRemainTime.Series[recordj[getrandom]].Points.Add(dp);
                }
                else
                {
                    dp = new DataPoint();
                    dp.BorderColor = Color.DarkGray;
                    dp.Color = c[recordj[getrandom]];
                    dp.XValue = thenumberOfJob[recordj[getrandom]] + 1;
                    int end = machineTime[thenumberOfJob[recordj[getrandom]]] + prepareData[recordj[getrandom]];
                    dp.YValues = new double[2] { machineTime[thenumberOfJob[recordj[getrandom]]], end }; // here we draw the chart

                    // here we record all time points
                    AllTimePoints.Add(machineTime[thenumberOfJob[recordj[getrandom]]]);
                    AllTimePoints.Add(end);
                    //AllTimePoints.Add(prepareData[recordj[getrandom]]);
                    // the jobtime, and the machine time need increate
                    machineTime[thenumberOfJob[recordj[getrandom]]] += prepareData[recordj[getrandom]];
                    jobTime[recordj[getrandom]] = machineTime[thenumberOfJob[recordj[getrandom]]]; //[recordj[getrandom]] record the number of select data
                    // here we draw the chart
                    chtRemainTime.Series[recordj[getrandom]].Points.Add(dp);
                }

                // here we remove the old data and add new data
                prepareData[recordj[getrandom]] = TimeCosts[recordj[getrandom]][0];
                thenumberOfJob[recordj[getrandom]] = JobSequence[recordj[getrandom]][0];
                TimeCosts[recordj[getrandom]].RemoveAt(0);
                JobSequence[recordj[getrandom]].RemoveAt(0);
                // check wether or not the data is empty
                if (JobSequence[recordj[getrandom]].Count == 0 && prepareData[recordj[getrandom]] != 0)
                {
                    TimeCosts[recordj[getrandom]].Add(0);
                    JobSequence[recordj[getrandom]].Add(11);
                }
            }
            rtbRemainTime.Clear();
            rtbRemainTime.AppendText($"the Maximum Span: {AllTimePoints.Max()}");
            AllTimePoints.Clear();
        }

        private void btnCombine_Click(object sender, EventArgs e)
        {
            // here we defined 2 matrix
            JobSequence = new List<List<int>>();
            TimeCosts = new List<List<int>>();
            for (int i = 0; i < jobs; i++)
            {
                List<int> timecost = new List<int>();
                List<int> sequence = new List<int>();
                for (int j = 0; j < machines; j++)
                {
                    sequence.Add(int.Parse(J[i][j].ToString()));
                    timecost.Add(int.Parse(T[i][j].ToString()));

                }
                JobSequence.Add(sequence);
                TimeCosts.Add(timecost);
            }
            // check how many data in our code
            machineTime = new int[machines];
            jobTime = new int[jobs];
            prepareData = new int[jobs];
            thenumberOfJob = new int[jobs];
            // here we get the data that we count
            for (int i = 0; i < jobs; i++)
            {
                // here set initial data
                jobTime[i] = 0;
                prepareData[i] = int.Parse(TimeCosts[i][0].ToString());
                thenumberOfJob[i] = int.Parse(JobSequence[i][0].ToString());
                JobSequence[i].RemoveAt(0);
                TimeCosts[i].RemoveAt(0);
            }
            for (int i = 0; i < machines; i++)
            {
                machineTime[i] = 0;
            }
            // clear data
            chtCombine.Series.Clear();
            // here we new the series
            for (int i = 0; i < jobs; i++)
            {
                Series s = new Series();
                s.ChartType = SeriesChartType.RangeBar;
                s.Color = c[i];
                s.Name = $"Job{i + 1}";
                chtCombine.Series.Add(s);
                chtCombine.Series[i].SetCustomProperty("DrawSideBySide", "False");
            }

            for (int i = 0; i < jobs * machines; i++)
            {
                int[] SumTotal = new int[jobs];
                int Max = 0;
                int Min = 100;
                for (int q = 0; q < jobs; q++)
                {
                    Min = 100;
                    if (Min > prepareData[q] & prepareData[q] != 0)
                    {
                        Min = prepareData[q];
                    }
                }
                for (int s = 0; s < jobs; s++)
                {
                    SumTotal[s] = TimeCosts[s].Sum() + prepareData[s];
                    if (SumTotal[s] > Max)
                    {
                        Max = SumTotal[s];
                    }
                }
                List<int> recordCost = new List<int>();
                List<int> recordNumber = new List<int>();
                List<int> recordj = new List<int>();
                for (int j = 0; j < jobs; j++)
                {

                    if (SumTotal[j] == Max || prepareData[j] == Min)
                    {
                        recordCost.Add(prepareData[j]);
                        recordNumber.Add(thenumberOfJob[j]); // record the machine number
                        recordj.Add(j); // record job number
                    }
                }
                int getrandom = r.Next(recordCost.Count);
                DataPoint dp;
                if (jobTime[recordj[getrandom]] >= machineTime[thenumberOfJob[recordj[getrandom]]])
                {
                    dp = new DataPoint();
                    dp.BorderColor = Color.DarkGray;
                    dp.Color = c[recordj[getrandom]];
                    dp.XValue = thenumberOfJob[recordj[getrandom]] + 1;
                    int end = jobTime[recordj[getrandom]] + prepareData[recordj[getrandom]];
                    dp.YValues = new double[2] { jobTime[recordj[getrandom]], end }; // here we draw the chart

                    // here we record all time points
                    AllTimePoints.Add(jobTime[recordj[getrandom]]);
                    AllTimePoints.Add(end);
                    AllTimePoints.Add(prepareData[recordj[getrandom]]);
                    // the jobtime, and the machine time need increate
                    jobTime[recordj[getrandom]] += prepareData[recordj[getrandom]]; //[recordj[getrandom]] record the number of select data
                    machineTime[thenumberOfJob[recordj[getrandom]]] = jobTime[recordj[getrandom]];
                    // here we draw the chart
                    chtCombine.Series[recordj[getrandom]].Points.Add(dp);
                }
                else
                {
                    dp = new DataPoint();
                    dp.BorderColor = Color.DarkGray;
                    dp.Color = c[recordj[getrandom]];
                    dp.XValue = thenumberOfJob[recordj[getrandom]] + 1;
                    int end = machineTime[thenumberOfJob[recordj[getrandom]]] + prepareData[recordj[getrandom]];
                    dp.YValues = new double[2] { machineTime[thenumberOfJob[recordj[getrandom]]], end }; // here we draw the chart

                    // here we record all time points
                    AllTimePoints.Add(machineTime[thenumberOfJob[recordj[getrandom]]]);
                    AllTimePoints.Add(end);
                    //AllTimePoints.Add(prepareData[recordj[getrandom]]);
                    // the jobtime, and the machine time need increate
                    machineTime[thenumberOfJob[recordj[getrandom]]] += prepareData[recordj[getrandom]];
                    jobTime[recordj[getrandom]] = machineTime[thenumberOfJob[recordj[getrandom]]]; //[recordj[getrandom]] record the number of select data
                    // here we draw the chart
                    chtCombine.Series[recordj[getrandom]].Points.Add(dp);
                }

                // here we remove the old data and add new data
                prepareData[recordj[getrandom]] = TimeCosts[recordj[getrandom]][0];
                thenumberOfJob[recordj[getrandom]] = JobSequence[recordj[getrandom]][0];
                TimeCosts[recordj[getrandom]].RemoveAt(0);
                JobSequence[recordj[getrandom]].RemoveAt(0);
                // check wether or not the data is empty
                if (JobSequence[recordj[getrandom]].Count == 0 && prepareData[recordj[getrandom]] != 0)
                {
                    TimeCosts[recordj[getrandom]].Add(0);
                    JobSequence[recordj[getrandom]].Add(11);
                }
            }
            rtbCombine.Clear();
            rtbCombine.AppendText($"the Maximum Span: {AllTimePoints.Max()}");
            AllTimePoints.Clear();
        }

        private void btnMachineLength_Click(object sender, EventArgs e)
        {

            // here we defined 2 matrix
            JobSequence = new List<List<int>>();
            TimeCosts = new List<List<int>>();
            int[] machineLength = new int[machines];
            List<int> selectmachineNumber = new List<int>();
            for (int i = 0; i < jobs; i++)
            {
                List<int> timecost = new List<int>();
                List<int> sequence = new List<int>();
                for (int j = 0; j < machines; j++)
                {
                    sequence.Add(int.Parse(J[i][j].ToString()));
                    timecost.Add(int.Parse(T[i][j].ToString()));

                }
                JobSequence.Add(sequence);
                TimeCosts.Add(timecost);
            }

            // check how many data in our code
            machineTime = new int[machines];
            jobTime = new int[jobs];
            prepareData = new int[jobs];
            thenumberOfJob = new int[jobs];
            // here we get the data that we count
            for (int i = 0; i < jobs; i++)
            {
                // here set initial data
                jobTime[i] = 0;
                prepareData[i] = int.Parse(TimeCosts[i][0].ToString());
                thenumberOfJob[i] = int.Parse(JobSequence[i][0].ToString());
                JobSequence[i].RemoveAt(0);
                TimeCosts[i].RemoveAt(0);
            }
            for (int i = 0; i < machines; i++)
            {
                machineTime[i] = 0;
            }
            // clear data
            chtMachineLength.Series.Clear();
            // here we new the series
            for (int i = 0; i < jobs; i++)
            {
                Series s = new Series();
                s.ChartType = SeriesChartType.RangeBar;
                s.Color = c[i];
                s.Name = $"Job{i + 1}";
                chtMachineLength.Series.Add(s);
                chtMachineLength.Series[i].SetCustomProperty("DrawSideBySide", "False");

            }
            // here we record the machines
            for (int i = 0; i < machines; i++)
            {
                machineLength[i] = jobs;
            }
            for (int i = 0; i < jobs * machines; i++)
            {
                int Max = 0;
                selectmachineNumber.Clear();
                for (int j = 0; j < machines; j++)
                {
                    if (machineLength[j] > Max)
                    {
                        Max = machineLength[j];
                    }
                }

                for (int j = 0; j < machines; j++)
                {
                    if (machineLength[j] == Max)
                    {
                        selectmachineNumber.Add(j);
                    }
                }
                List<int> recordCost = new List<int>();
                List<int> recordNumber = new List<int>();
                List<int> recordj = new List<int>();
                for (int j = 0; j < jobs; j++)
                {
                    //if(thenumberOfJob[j].Contains(selectmachineNumber))  
                    if (selectmachineNumber.Contains(thenumberOfJob[j]))
                    {
                        recordCost.Add(prepareData[j]);
                        recordNumber.Add(thenumberOfJob[j]); // record the machine number
                        recordj.Add(j); // record job number
                    }
                }
                // here if recordCost count is null
                if (recordCost.Count == 0)
                {
                    for (int s = 0; s < machines - 1; s++)
                    {
                        Max--;
                        selectmachineNumber.Clear();
                        for (int m = 0; m < machines; m++)
                        {
                            if (machineLength[m] == Max)
                            {
                                selectmachineNumber.Add(m);
                            }
                        }
                        recordCost = new List<int>();
                        recordNumber = new List<int>();
                        recordj = new List<int>();
                        for (int j = 0; j < jobs; j++)
                        {
                            //if(thenumberOfJob[j].Contains(selectmachineNumber))  
                            if (selectmachineNumber.Contains(thenumberOfJob[j]))
                            {
                                recordCost.Add(prepareData[j]);
                                recordNumber.Add(thenumberOfJob[j]); // record the machine number
                                recordj.Add(j); // record job number
                            }
                        }
                        if (recordCost.Count != 0)
                        {
                            break;
                        }
                    }
                }
                int getrandom = r.Next(recordCost.Count);
                machineLength[thenumberOfJob[recordj[getrandom]]]--;
                DataPoint dp;
                if (jobTime[recordj[getrandom]] >= machineTime[thenumberOfJob[recordj[getrandom]]])
                {
                    dp = new DataPoint();
                    dp.BorderColor = Color.DarkGray;
                    dp.Color = c[recordj[getrandom]];
                    dp.XValue = thenumberOfJob[recordj[getrandom]] + 1;
                    int end = jobTime[recordj[getrandom]] + prepareData[recordj[getrandom]];
                    dp.YValues = new double[2] { jobTime[recordj[getrandom]], end }; // here we draw the chart

                    // here we record all time points
                    AllTimePoints.Add(jobTime[recordj[getrandom]]);
                    AllTimePoints.Add(end);
                    AllTimePoints.Add(prepareData[recordj[getrandom]]);
                    // the jobtime, and the machine time need increate
                    jobTime[recordj[getrandom]] += prepareData[recordj[getrandom]]; //[recordj[getrandom]] record the number of select data
                    machineTime[thenumberOfJob[recordj[getrandom]]] = jobTime[recordj[getrandom]];
                    // here we draw the chart
                    chtMachineLength.Series[recordj[getrandom]].Points.Add(dp);
                }
                else
                {
                    dp = new DataPoint();
                    dp.BorderColor = Color.DarkGray;
                    dp.Color = c[recordj[getrandom]];
                    dp.XValue = thenumberOfJob[recordj[getrandom]] + 1;
                    int end = machineTime[thenumberOfJob[recordj[getrandom]]] + prepareData[recordj[getrandom]];
                    dp.YValues = new double[2] { machineTime[thenumberOfJob[recordj[getrandom]]], end }; // here we draw the chart

                    // here we record all time points
                    AllTimePoints.Add(machineTime[thenumberOfJob[recordj[getrandom]]]);
                    AllTimePoints.Add(end);
                    //AllTimePoints.Add(prepareData[recordj[getrandom]]);
                    // the jobtime, and the machine time need increate
                    machineTime[thenumberOfJob[recordj[getrandom]]] += prepareData[recordj[getrandom]];
                    jobTime[recordj[getrandom]] = machineTime[thenumberOfJob[recordj[getrandom]]]; //[recordj[getrandom]] record the number of select data
                    // here we draw the chart
                    chtMachineLength.Series[recordj[getrandom]].Points.Add(dp);
                }

                // here we remove the old data and add new data
                prepareData[recordj[getrandom]] = TimeCosts[recordj[getrandom]][0];
                thenumberOfJob[recordj[getrandom]] = JobSequence[recordj[getrandom]][0];
                TimeCosts[recordj[getrandom]].RemoveAt(0);
                JobSequence[recordj[getrandom]].RemoveAt(0);
                // check wether or not the data is empty
                if (JobSequence[recordj[getrandom]].Count == 0 && prepareData[recordj[getrandom]] != 100)
                {
                    TimeCosts[recordj[getrandom]].Add(100);
                    JobSequence[recordj[getrandom]].Add(11);
                }
            }
            rtbMachineLength.Clear();
            rtbMachineLength.AppendText($"the Maximum Span: {AllTimePoints.Max()}");
            AllTimePoints.Clear();
        }

        private void btnMachineTime_Click(object sender, EventArgs e)
        {
            ///// bug/////
            // here we defined 2 matrix
            JobSequence = new List<List<int>>();
            TimeCosts = new List<List<int>>();
            List<List<int>> machineLongestTime = new List<List<int>>();
            int[] machinetotalTime = new int[machines];
            List<int> selectmachineNumber = new List<int>();
            for (int i = 0; i < jobs; i++)
            {
                List<int> timecost = new List<int>();
                List<int> sequence = new List<int>();
                for (int j = 0; j < machines; j++)
                {
                    sequence.Add(int.Parse(J[i][j].ToString()));
                    timecost.Add(int.Parse(T[i][j].ToString()));

                }
                JobSequence.Add(sequence);
                TimeCosts.Add(timecost);
            }

            // check how many data in our code
            machineTime = new int[machines];
            jobTime = new int[jobs];
            prepareData = new int[jobs];
            thenumberOfJob = new int[jobs];

            // here we get the data that we count
            for (int z = 0; z < machines; z++)
            {
                List<int> longest = new List<int>();
                for (int t = 0; t < jobs; t++)
                {
                    for (int o = 0; o < machines; o++)
                    {
                        if (z == int.Parse(JobSequence[t][o].ToString()))
                        {
                            longest.Add(int.Parse(TimeCosts[t][o].ToString()));
                        }
                    }
                }
                machineLongestTime.Add(longest);
            }
            for (int i = 0; i < jobs; i++)
            {
                // here set initial data
                jobTime[i] = 0;
                prepareData[i] = int.Parse(TimeCosts[i][0].ToString());
                thenumberOfJob[i] = int.Parse(JobSequence[i][0].ToString());
                JobSequence[i].RemoveAt(0);
                TimeCosts[i].RemoveAt(0);
            }
            for (int i = 0; i < machines; i++)
            {
                machineTime[i] = 0;
            }
            // clear data
            chtMachineTime.Series.Clear();
            // here we new the series
            for (int i = 0; i < jobs; i++)
            {
                Series s = new Series();
                s.ChartType = SeriesChartType.RangeBar;
                s.Color = c[i];
                s.Name = $"Job{i + 1}";
                chtMachineTime.Series.Add(s);
                chtMachineTime.Series[i].SetCustomProperty("DrawSideBySide", "False");

            }
            // here we record the machines
            for (int i = 0; i < jobs * machines; i++)
            {
                int Max = 0;
                List<int> maxindex = new List<int>();
                selectmachineNumber.Clear();

                for (int m = 0; m < machines; m++)
                {
                    machinetotalTime[m] = 0;
                }
                for (int j = 0; j < machines; j++)
                {

                    machinetotalTime[j] = machineLongestTime[j].Sum();
                    if (machinetotalTime[j] > Max)
                    {
                        Max = machinetotalTime[j];
                    }
                }
                for (int j = 0; j < machines; j++)
                {
                    if (machinetotalTime[j] == Max)
                    {
                        selectmachineNumber.Add(j);
                        maxindex.Add(j);
                    }
                }
                List<int> recordCost = new List<int>();
                List<int> recordNumber = new List<int>();
                List<int> recordj = new List<int>();
                for (int j = 0; j < jobs; j++)
                {
                    if (selectmachineNumber.Contains(thenumberOfJob[j]))
                    {
                        recordCost.Add(prepareData[j]);
                        recordNumber.Add(thenumberOfJob[j]); // record the machine number
                        recordj.Add(j); // record job number
                    }
                }
                if (recordCost.Count == 0)
                {
                    do
                    {
                        Max = 0;
                        int index = 0;
                        for (int m = 0; m < machines; m++)
                        {
                            if (maxindex.Contains(m))
                            {
                            }
                            else
                            {
                                if (machinetotalTime[m] > Max)
                                {
                                    Max = machinetotalTime[m];
                                    index = m;
                                }
                            }
                        }
                        for (int s = 0; s < machines; s++)
                        {
                            if (machinetotalTime[s] == Max)
                            {
                                selectmachineNumber.Add(s);
                            }
                        }
                        maxindex.Add(index);
                        recordCost = new List<int>();
                        recordNumber = new List<int>();
                        recordj = new List<int>();
                        for (int l = 0; l < jobs; l++)
                        {
                            if (selectmachineNumber.Contains(thenumberOfJob[l]))
                            {
                                recordCost.Add(prepareData[l]);
                                recordNumber.Add(thenumberOfJob[l]);
                                recordj.Add(l);
                            }
                            if (recordCost.Count != 0)
                            {
                                break;
                            }
                        }

                    } while (recordCost.Count == 0);
                }
                int getrandom = r.Next(recordCost.Count);
                machineLongestTime[recordNumber[getrandom]][recordj[getrandom]] = 0;
                DataPoint dp;
                if (jobTime[recordj[getrandom]] >= machineTime[thenumberOfJob[recordj[getrandom]]])
                {
                    dp = new DataPoint();
                    dp.BorderColor = Color.DarkGray;
                    dp.Color = c[recordj[getrandom]];
                    dp.XValue = thenumberOfJob[recordj[getrandom]] + 1;
                    int end = jobTime[recordj[getrandom]] + prepareData[recordj[getrandom]];
                    dp.YValues = new double[2] { jobTime[recordj[getrandom]], end }; // here we draw the chart

                    // here we record all time points
                    AllTimePoints.Add(jobTime[recordj[getrandom]]);
                    AllTimePoints.Add(end);
                    AllTimePoints.Add(prepareData[recordj[getrandom]]);
                    // the jobtime, and the machine time need increate
                    jobTime[recordj[getrandom]] += prepareData[recordj[getrandom]]; //[recordj[getrandom]] record the number of select data
                    machineTime[thenumberOfJob[recordj[getrandom]]] = jobTime[recordj[getrandom]];
                    // here we draw the chart
                    chtMachineTime.Series[recordj[getrandom]].Points.Add(dp);
                }
                else
                {
                    dp = new DataPoint();
                    dp.BorderColor = Color.DarkGray;
                    dp.Color = c[recordj[getrandom]];
                    dp.XValue = thenumberOfJob[recordj[getrandom]] + 1;
                    int end = machineTime[thenumberOfJob[recordj[getrandom]]] + prepareData[recordj[getrandom]];
                    dp.YValues = new double[2] { machineTime[thenumberOfJob[recordj[getrandom]]], end }; // here we draw the chart

                    // here we record all time points
                    AllTimePoints.Add(machineTime[thenumberOfJob[recordj[getrandom]]]);
                    AllTimePoints.Add(end);
                    //AllTimePoints.Add(prepareData[recordj[getrandom]]);
                    // the jobtime, and the machine time need increate
                    machineTime[thenumberOfJob[recordj[getrandom]]] += prepareData[recordj[getrandom]];
                    jobTime[recordj[getrandom]] = machineTime[thenumberOfJob[recordj[getrandom]]]; //[recordj[getrandom]] record the number of select data
                    // here we draw the chart
                    chtMachineTime.Series[recordj[getrandom]].Points.Add(dp);
                }

                // here we remove the old data and add new data
                prepareData[recordj[getrandom]] = TimeCosts[recordj[getrandom]][0];
                thenumberOfJob[recordj[getrandom]] = JobSequence[recordj[getrandom]][0];
                TimeCosts[recordj[getrandom]].RemoveAt(0);
                JobSequence[recordj[getrandom]].RemoveAt(0);
                //machineLongestTime[recordj[getrandom]].RemoveAt(0);
                // check wether or not the data is empty
                if (JobSequence[recordj[getrandom]].Count == 0 && prepareData[recordj[getrandom]] != 100)
                {
                    TimeCosts[recordj[getrandom]].Add(100);
                    JobSequence[recordj[getrandom]].Add(11);
                }
            }
            rtbMachineTime.Clear();
            rtbMachineTime.AppendText($"the Maximum Span: {AllTimePoints.Max()}");
            AllTimePoints.Clear();
        }

        private void btnGeneticAlgorithm_Click(object sender, EventArgs e)
        {

            // data fileds
            List<List<int>> populationlist = new List<List<int>>();
            List<double> makespanrecord = new List<double>();
            int populationsize = int.Parse(tbPopulationSize.Text);
            int iteration = int.Parse(tbIteration.Text);
            double crossoverrate = double.Parse(tbCrossoverRate.Text);
            double mutationrate = double.Parse(tbMutationRate.Text);
            int Tbest = 99999999;
            List<int> bestroute = new List<int>();
            rtbGeneticAlgorithm.Clear(); // clear the data

            // clear data
            chtGeneticAlgorithm.Series.Clear();

            // here we defined 2 matrix
            JobSequence = new List<List<int>>();
            TimeCosts = new List<List<int>>();
            for (int i = 0; i < jobs; i++)
            {
                List<int> timecost = new List<int>();
                List<int> sequence = new List<int>();
                for (int j = 0; j < machines; j++)
                {
                    sequence.Add(int.Parse(J[i][j].ToString()));
                    timecost.Add(int.Parse(T[i][j].ToString()));

                }
                JobSequence.Add(sequence);
                TimeCosts.Add(timecost);
            }

            // here we new the series
            for (int i = 0; i < jobs; i++)
            {
                Series s = new Series();
                s.ChartType = SeriesChartType.RangeBar;
                s.Color = c[i];
                s.Name = $"Job{i + 1}";
                chtGeneticAlgorithm.Series.Add(s);
                chtGeneticAlgorithm.Series[i].SetCustomProperty("DrawSideBySide", "False");
            }

            // Initial population
            for (int i = 0; i < populationsize; i++)
            {
                List<int> randompermutation = new List<int>();
                for (int n = 0; n < machines * jobs; n++)
                {

                    int rnum = r.Next(0, machines * jobs);
                    if (randompermutation.Contains(rnum))
                    {
                        n--;
                    }
                    else
                    {
                        randompermutation.Add(rnum);
                    }
                }
                populationlist.Add(randompermutation);
                for (int j = 0; j < machines * jobs; j++)
                {
                    populationlist[i][j] = populationlist[i][j] % jobs;
                }
            }
            for (int n = 0; n < iteration; n++) // here is the iteration
            {
                int Tbestnow = 9999999;
                // two point crossover
                // data fileds
                List<int> parent_1 = new List<int>();
                List<int> parent_2 = new List<int>();

                List<List<int>> parentlist = populationlist;
                List<List<int>> offspringlist = new List<List<int>>();
                offspringlist.AddRange(parentlist);
                List<int> S = new List<int>();
                for (int v = 0; v < populationsize; v++)
                {
                    int rndS = r.Next(populationsize);
                    if (S.Contains(rndS))
                    {
                        v--;
                    }
                    else
                    {
                        S.Add(rndS);
                    }

                }
                // here is the operate
                for (int i = 0; i < populationsize / 2; i++)
                {
                    double crossoverprob = r.NextDouble();
                    if (crossoverrate >= crossoverprob)
                    {
                        List<int> child_1 = new List<int>();
                        List<int> child_2 = new List<int>();
                        parent_1 = populationlist[S[2 * i]];
                        parent_2 = populationlist[S[2 * i + 1]];
                        child_1.AddRange(parent_1);
                        child_2.AddRange(parent_2);
                        List<int> cutpoint = new List<int>();
                        cutpoint.Add(r.Next(jobs * machines));
                        cutpoint.Add(r.Next(jobs * machines));
                        cutpoint.Sort();
                        for (int j = cutpoint[0]; j < cutpoint[1]; j++)
                        {
                            int p_1 = parent_1[j];
                            int p_2 = parent_2[j];
                            child_1[j] = p_2;
                            child_2[j] = p_1;
                        }
                        offspringlist.RemoveAt(S[2 * i]);
                        offspringlist.Insert(S[2 * i], child_1);
                        offspringlist.RemoveAt(S[2 * i + 1]);
                        offspringlist.Insert(S[2 * i + 1], child_2);
                    }

                }


                // repairment
                for (int m = 0; m < populationsize; m++)
                {
                    // data fileds
                    //Dictionary<int, int> jobcount = new Dictionary<int, int>();
                    List<List<int>> pos = new List<List<int>>();
                    List<int> larger = new List<int>();
                    List<List<int>> largerpos = new List<List<int>>();
                    List<int> less = new List<int>();
                    List<List<int>> lesspos = new List<List<int>>();
                    // operate
                    for (int i = 0; i < jobs; i++)
                    {
                        List<int> p = new List<int>();
                        for (int j = 0; j < offspringlist[m].Count; j++)
                        {
                            if (offspringlist[m][j] == i)
                            {
                                p.Add(j);
                            }

                        }
                        pos.Add(p);
                        if (pos[i].Count > machines)
                        {
                            larger.Add(i); // here show how many larger number
                            largerpos.Add(p); // here we record the 
                        }
                        else if (pos[i].Count < machines)
                        {
                            less.Add(i); // here show how many number of less number
                            lesspos.Add(p);
                        }
                    }
                    
                    for (int d = 0; d < less.Count; d++)
                    {
                        if (lesspos[d].Count == machines)
                        {
                            less.RemoveAt(0);
                            lesspos.RemoveAt(0);
                        }
                        if (lesspos.Count == 0 || largerpos.Count == 0)
                        {
                            break;
                        }
                        if (lesspos[d].Count < machines)
                        {
                            if (largerpos.Count == 0)
                            {
                                break;
                            }
                            else
                            {
                                if (largerpos[0].Count == machines)
                                {
                                    larger.RemoveAt(0);
                                    largerpos.RemoveAt(0);
                                }
                                else
                                {
                                    int exp;
                                    if (lesspos[d].Count == 0)
                                    {
                                        lesspos[d].Add(offspringlist[m][largerpos[d][0]]);
                                        largerpos[d].RemoveAt(0);
                                    }
                                    else
                                    {
                                        exp = offspringlist[m][lesspos[d][0]];
                                        offspringlist[m][largerpos[d][0]] = exp;
                                        lesspos[d].Add(offspringlist[m][largerpos[d][0]]);
                                        //lesspos.Add(offspringlist[m][largerpos[d]]);
                                        largerpos[d].RemoveAt(0);
                                    }
                                    
                                }
                                d--;
                            }
                        }

                    }
                }


                // mutation
                // data fileds
                int nummutationjob = (int)(mutationrate * machines * jobs);
                // operate
                for (int m = 0; m < offspringlist.Count; m++)
                {
                    double mutationprob = r.NextDouble();
                    if (mutationrate >= mutationprob)
                    {
                        List<int> mchg = new List<int>();
                        for (int i = 0; i < nummutationjob; i++)
                        {
                            int ll = r.Next(machines * jobs);
                            if (mchg.Contains(ll))
                            {
                                i--;
                            }
                            else
                            {
                                mchg.Add(ll);
                            }

                        }
                        int tvaluelast = offspringlist[m][mchg[0]];
                        for (int i = 0; i < nummutationjob - 1; i++)
                        {
                            int exp = offspringlist[m][mchg[i + 1]];
                            offspringlist[m][mchg[i]] = exp;
                        }
                        offspringlist[m][mchg[nummutationjob - 1]] = tvaluelast;
                    }
                }


                



                //fitness value(calculate makespan)
                // data fileds
                List<List<int>> totalchromosome = new List<List<int>>();
                totalchromosome.AddRange(parentlist);
                totalchromosome.AddRange(offspringlist);
                double totalfitness = 0.0;
                List<double> chromfitness = new List<double>();
                List<int> chromfit = new List<int>();
                // operate
                for (int m = 0; m < populationsize * 2; m++)
                {

                    List<int> jkeys = new List<int>();
                    List<int> keycount = new List<int>();
                    List<int> jcount = new List<int>();
                    List<int> mkeys = new List<int>();
                    List<int> mcount = new List<int>();
                    for (int j = 0; j < jobs; j++)
                    {
                        jkeys.Add(j); // 0 - 9
                        keycount.Add(0);
                        jcount.Add(0);

                    }
                    for (int j = 0; j < machines; j++)
                    {
                        mkeys.Add(j); //0 - 9 
                        mcount.Add(0);
                    }
                    for (int i = 0; i < totalchromosome[m].Count; i++)
                    {
                        int station = totalchromosome[m][i];
                        int gent = TimeCosts[station][keycount[station]];
                        //int genm = JobSequence[station][keycount[station]];
                        int genm = JobSequence[station][keycount[station]];
                        jcount[station] = jcount[station] + gent;
                        mcount[genm] = mcount[genm] + gent;
                        if (mcount[genm] < jcount[station])
                        {
                            mcount[genm] = jcount[station];
                        }
                        else if (mcount[genm] > jcount[station])
                        {
                            jcount[station] = mcount[genm];
                        }
                        keycount[station] = keycount[station] + 1;
                    }
                    int makespan = 0;
                    for (int s = 0; s < jcount.Count; s++)
                    {
                        if (makespan < jcount[s])
                        {
                            makespan = jcount[s];
                        }
                    }
                    chromfitness.Add(1.0 / makespan);
                    chromfit.Add(makespan);
                    totalfitness = totalfitness + chromfitness[m];
                }


                // selection(rouette wheel approach)
                // data fileds
                List<double> pk = new List<double>();
                List<double> qk = new List<double>();
                List<double> selectionrand = new List<double>();
                // functions
                for (int i = 0; i < populationsize * 2; i++)
                {
                    pk.Add(chromfitness[i] / totalfitness);
                }
                for (int i = 0; i < populationsize * 2; i++)
                {
                    double cumulative = 0;
                    for (int j = 0; j < i + 1; j++)
                    {
                        cumulative = cumulative + pk[j];
                    }
                    qk.Add(cumulative);
                }
                for (int i = 0; i < populationsize; i++)
                {
                    selectionrand.Add(r.NextDouble());
                }
                for (int i = 0; i < populationsize; i++)
                {
                    if (selectionrand[i] < qk[0])
                    {
                        populationlist[i] = totalchromosome[0];
                    }
                    else
                    {
                        for (int j = 0; j < populationsize * 2 - 1; j++)
                        {
                            if (selectionrand[i] > qk[j] && selectionrand[i] <= qk[j + 1])
                            {
                                populationlist[i] = totalchromosome[j + 1];
                                break;
                            }
                        }
                    }
                }


                // comparison
                // data fileds
                // operate
                List<int> sequencenow = new List<int>();
                List<int> sequencebest = new List<int>();

                for (int i = 0; i < populationsize * 2; i++)
                {
                    if (chromfit[i] < Tbestnow)
                    {
                        Tbestnow = chromfit[i];
                        sequencenow = totalchromosome[i];
                    }
                }
                if (Tbestnow <= Tbest)
                {
                    Tbest = Tbestnow;
                    sequencebest = sequencenow;
                    bestroute = sequencebest;
                }
                makespanrecord.Add(Tbest);

            }

            for (int i = 0; i < makespanrecord.Count; i++)
            {
                string nnn = makespanrecord[i].ToString();
                if (i == makespanrecord.Count - 1)
                {
                    rtbGeneticAlgorithm.AppendText("\nBest Result\n");
                    rtbGeneticAlgorithm.AppendText(nnn);
                }
                else
                {
                    rtbGeneticAlgorithm.AppendText(nnn);
                }
            }
            rtbGeneticAlgorithm.AppendText("\nBest Sequence\n");
            rtbGeneticAlgorithm.AppendText("[");
            for (int i = 0; i < jobs * machines; i++)
            {
                rtbGeneticAlgorithm.AppendText(bestroute[i].ToString());
            }
            rtbGeneticAlgorithm.AppendText("]");
            // draw the chart && show the result 
            machineTime = new int[machines];
            jobTime = new int[jobs];
            prepareData = new int[jobs];
            thenumberOfJob = new int[jobs];
            for (int i = 0; i < jobs; i++)
            {
                jobTime[i] = 0;
                prepareData[i] = int.Parse(TimeCosts[i][0].ToString());
                thenumberOfJob[i] = int.Parse(JobSequence[i][0].ToString());
                JobSequence[i].RemoveAt(0);
                TimeCosts[i].RemoveAt(0);
            }
            for (int i = 0; i < machines; i++)
            {
                machineTime[i] = 0;
            }
            for (int i = 0; i < jobs * machines; i++)
            {
                DataPoint dp;
                int recordj = 0;
                recordj = bestroute[i];
                if(jobTime[recordj] >= machineTime[thenumberOfJob[recordj]])
                {
                    dp = new DataPoint();
                    dp.BorderColor = Color.DarkGray;
                    dp.Color = c[recordj];
                    dp.XValue = thenumberOfJob[recordj] + 1;
                    int end = jobTime[recordj] + prepareData[recordj];
                    dp.YValues = new double[2] { jobTime[recordj], end }; // here we draw the chart
                    AllTimePoints.Add(jobTime[recordj]);
                    AllTimePoints.Add(end);
                    AllTimePoints.Add(prepareData[recordj]);
                    // the jobtime, and the machine time need increate
                    jobTime[recordj] += prepareData[recordj]; //[recordj[getrandom]] record the number of select data
                    machineTime[thenumberOfJob[recordj]] = jobTime[recordj];
                    // here we draw the chart
                    chtGeneticAlgorithm.Series[recordj].Points.Add(dp);
                }
                else
                {
                    dp = new DataPoint();
                    dp.BorderColor = Color.DarkGray;
                    dp.Color = c[recordj];
                    dp.XValue = thenumberOfJob[recordj] + 1;
                    int end = machineTime[thenumberOfJob[recordj]] + prepareData[recordj];
                    dp.YValues = new double[2] { machineTime[thenumberOfJob[recordj]], end }; // here we draw the chart

                    // here we record all time points
                    AllTimePoints.Add(machineTime[thenumberOfJob[recordj]]);
                    AllTimePoints.Add(end);
                    //AllTimePoints.Add(prepareData[recordj[getrandom]]);
                    // the jobtime, and the machine time need increate
                    machineTime[thenumberOfJob[recordj]] += prepareData[recordj];
                    jobTime[recordj] = machineTime[thenumberOfJob[recordj]]; //[recordj[getrandom]] record the number of select data
                    // here we draw the chart
                    chtGeneticAlgorithm.Series[recordj].Points.Add(dp);
                }
                prepareData[recordj] = TimeCosts[recordj][0];
                thenumberOfJob[recordj] = JobSequence[recordj][0];
                TimeCosts[recordj].RemoveAt(0);
                JobSequence[recordj].RemoveAt(0);
                // check wether or not the data is empty
                if (JobSequence[recordj].Count == 0 && prepareData[recordj] != 100)
                {
                    TimeCosts[recordj].Add(100);
                    JobSequence[recordj].Add(11);
                }
            }

            }

        









    }




}
