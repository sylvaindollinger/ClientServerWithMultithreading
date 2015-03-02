using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace CommonLibrary
{
   
    public class Job
    {

        static public List<Job> RunningJobs = new List<Job>();
        public const int RunningJobsMaxLimit = 20;

        public UInt64 id;       //identifiant attribué par le client
        public byte priorite;   //0..3
        public byte type;       //1..4
        public UInt32 duree;    //millisecond

        public const int bufferSize = 8 + 1 + 1 + 4;

        //------------------------------------------
        // Constructors / Destructors
        //------------------------------------------

    

        public Job(UInt64 id, byte priorite, byte type, UInt32 duree) 
        {
            this.id = id;
            this.priorite = priorite;
            this.type = type;
            this.duree = duree;
        }


        //------------------------------------------
        // ToString
        //------------------------------------------

        override public String ToString()
        {
            
            String details = "id=" + BitConverter.ToString(BitConverter.GetBytes(id)) + ",priorite=" + priorite + ",type=" + type + ",duree=" + duree;
            return details;
        }


        //------------------------------------------
        // Create and return new Traitement from buffer
        //------------------------------------------

        static public Job FromByte(byte[] buffer)
        {
            UInt64 id;        //identifiant attribué par le client
            byte priorite;    //0..3
            byte type;        //1..4
            UInt32 duree;     //millisecond

            id = BitConverter.ToUInt64(buffer, 0);
            priorite = buffer[8];
            type = buffer[9];
            duree = BitConverter.ToUInt32(buffer, 10); ;

            Job temp = new Job(id, priorite, type, duree);
            return temp;
        }


        public byte[] ToByte()
        {
            byte[] buffer = new byte[bufferSize];
            BitConverter.GetBytes(id).CopyTo(buffer, 0);
            BitConverter.GetBytes(priorite).CopyTo(buffer, 8);
            BitConverter.GetBytes(type).CopyTo(buffer, 9);
            BitConverter.GetBytes(duree).CopyTo(buffer, 10);
            return buffer;
        }


        //-----------------------------------
        //
        // MULTITHREADING
        //
        //-----------------------------------

        public bool TryStart()
        {
            bool rep = false;
            Monitor.Enter(RunningJobs);
               // lock (this)
            {

                if (RunningJobs.Count < RunningJobsMaxLimit)
                {
                    //Console.WriteLine("--> [{0}]", this.ToString());
                    RunningJobs.Add(this);
                    //Console.WriteLine("Threads({0})", Job.RunningJobs.Count);
                    Console.WriteLine("[{1}] --> Threads({0})", Job.RunningJobs.Count, this.ToString());
                    Monitor.Exit(RunningJobs);

                    Thread thread = new Thread(new ThreadStart(Run));
                    thread.Start();
                    rep = true;
                }
                else
                    {
                    Monitor.Exit(RunningJobs);
                    rep = false;
                    }
                
            }
            return rep;
        }


        private void Run()
        {
            Thread.Sleep((int)this.duree);
            //lock (this)
                //Monitor.Enter(RunningJobs);
            {
                Monitor.Enter(RunningJobs);
                //Console.WriteLine("<-- [{0}]", this.ToString());
                RunningJobs.Remove(this);
                Console.WriteLine("[{1}] <-- Threads({0})", Job.RunningJobs.Count,this.ToString());
                Monitor.Exit(RunningJobs);

            }
            
        }
    }




}

