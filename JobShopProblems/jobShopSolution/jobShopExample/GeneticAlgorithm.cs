using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jobShopExample
{
    class GeneticAlgorithm
    {
        // data fileds
        double crossoverrate = 0.8;
        double mutationrate = 0.2;
        int iteration = 100;
        int populationsize = 30;
        List<List<int>> populationlist = new List<List<int>>();
        Random r = new Random();

        // Properites
        public GeneticAlgorithm()
        {

        }

        // here we initial the data
        public List<List<int>> Initialpopulation(int machine, int job)
        {
            for (int i = 0; i < populationsize; i++)
            {
                populationlist[i].Add(r.Next(0, machine * job));
                for (int j = 0; j < machine * job; j++)
                {
                    populationlist[i][j] = populationlist[i][j] % job;
                }
            }
            return populationlist;
        }
        // here we get the result that get the max Span of this method 
        public double GetResult()
        {
            return 0;
        }

        // here we crossover the data
        public List<List<int>> Crossover(List<List<int>> populationlist, int job, int machine)
        {
            List<int> parent_1 = new List<int>();
            List<int> parent_2 = new List<int>();
            List<int> child_1 = new List<int>();
            List<int> child_2 = new List<int>();
            List<List<int>> offspringlist = new List<List<int>>();
            List<int> cutpoint = new List<int>();
            for (int i = 0; i < populationsize / 2; i++)
            {
                double crossoverprob = r.NextDouble();
                if (crossoverrate >= crossoverprob)
                {
                    for (int s = 0; s < job; s++)
                    {
                        parent_1 = populationlist[2 * i];
                        parent_2 = populationlist[2 * i + 1];
                    }
                    child_1 = parent_1;
                    child_2 = parent_2;
                    cutpoint[0] = r.Next(job * machine);
                    cutpoint[1] = r.Next(job * machine);
                    cutpoint.Sort();
                    for (int j = cutpoint[0]; j < cutpoint[1]; j++)
                    {
                        child_1[j] = parent_2[j];
                        child_2[j] = parent_1[j];
                    }
                    offspringlist[2 * i] = child_1;
                    offspringlist[2 * i + 1] = child_2;
                }
            }
            return offspringlist;
        }
        // repair the missed data
        public void Repairment(int job, int machine, List<List<int>> offspringlist)
        {
            for (int m = 0; m < populationsize; m++)
            {
                List<List<int>> jobcount = new List<List<int>>();
                List<int> larger = new List<int>();
                List<int> less = new List<int>();
                for (int i = 0; i < job; i++)
                {
                    int count;
                    if (offspringlist[m].Contains(i))
                    {
                        count = 0;
                        for (int j = 0; j < offspringlist.Count; j++)
                        {
                            if (offspringlist[m].Contains(i))
                            {
                                count++;
                            }
                        }
                        jobcount[i].Add(count);
                        jobcount[i].Add(offspringlist[m].IndexOf(i));
                    }
                    else
                    {
                        count = 0;
                        jobcount[i].Add(count);
                        jobcount[i].Add(0);
                    }
                    if (count > machine)
                    {
                        larger.Add(i);
                    }
                    else if (count < machine)
                    {
                        less.Add(i);
                    }
                }
                for (int k = 0; k < larger.Count; k++)
                {
                    int chgjob = larger[k];
                    while(jobcount[chgjob][0] > machine)
                    {
                        for (int d = 0; d < less.Count; d++)
                        {
                            if(jobcount[less[d]][0] < machine)
                            {
                                offspringlist[m][jobcount[chgjob][1]] = less[d];
                                jobcount[chgjob][1] = offspringlist[m].IndexOf(chgjob);
                                jobcount[chgjob][0] = jobcount[chgjob][0] - 1;
                                jobcount[chgjob][0] = jobcount[less[d]][0] + 1;
                                
                            }
                            if(jobcount[chgjob][0] == machine)
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }
        // mutation
        public void Mutation(int job, int machine, List<List<int>> offsprintlist)
        {
            int nummutationjob = (int)(mutationrate * machine * job);
            for (int m = 0; m < offsprintlist.Count; m++)
            {
                double mutationprob = r.Next();
                if (mutationrate >= mutationprob)
                {
                    List<int> mchg = new List<int>();
                    for (int i = 0; i < nummutationjob; i++)
                    {
                        mchg.Add(r.Next(machine * job));
                    }
                    int tvaluelast = offsprintlist[m][mchg[0]];
                    for (int i = 0; i < nummutationjob - 1; i++)
                    {
                        offsprintlist[m][mchg[i]] = offsprintlist[m][mchg[m + 1]];
                    }
                    offsprintlist[m][mchg[nummutationjob - 1]] = tvaluelast;
                }
            }
        }
        // fitness value 
        public void Fitness(int job, int machine, List<List<int>> pt, List<List<int>> ms)
        {
            List<List<int>> totalchromosome = new List<List<int>>();
            //double totalfitness = 0.0;
            for (int m = 0; m < populationsize; m++)
            {

                for (int i = 0; i < totalchromosome[m].Count; i++)
                {

                    

                }

            }





        }
        // selection
        public void Selection()
        {

        }


        // comparison
         public void Comparison()
        {
            for (int i = 0; i < populationsize * 2; i++)
            {
                

            }
            List<List<int>> offspringlist = new List<List<int>>();
            List<List<int>> parentlist = new List<List<int>>();
            
            int[] a = new int[10];
            int[] b = new int[10];
            int a_ = 0;

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    if (offspringlist[0][j] == i)
                    {
                        a[i] += 1;

                    }
                    if (parentlist[0][j] == i)
                    {
                        b[i] += 1;
                        if (b[i] == a[i])
                        {
                            a_++;
                        }
                    }

                }

            }


        }


    }
}
