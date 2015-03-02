using CommonLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;


namespace ServeurMultithread
{


    public class Scheduler
    {

        //ThreadPool threadpool = null;

        Queue<Job> fileAttenteP0 = null;
        Queue<Job> fileAttenteP1 = null;
        Queue<Job> fileAttenteP2 = null;
        Queue<Job> fileAttenteP3 = null;

        public Scheduler()
        {
            //threadpool = new ThreadPool.
            fileAttenteP0 = new Queue<Job>();
            fileAttenteP1 = new Queue<Job>();
            fileAttenteP2 = new Queue<Job>();
            fileAttenteP3 = new Queue<Job>();
        }

      
        //-----------------------------------
        //
        // Queuing
        //
        //-----------------------------------


        public void Enqueue(byte[] buffer)
        {
            Job traitement = Job.FromByte(buffer);

            //Console.WriteLine("Threads({0})  <-- [{0}]", traitement.ToString());
            Console.WriteLine("[{0}] Queued", traitement.ToString());
            switch (traitement.priorite)
            {
                case 0: fileAttenteP0.Enqueue(traitement); break;
                case 1: fileAttenteP1.Enqueue(traitement); break;
                case 2: fileAttenteP2.Enqueue(traitement); break;
                case 3: fileAttenteP3.Enqueue(traitement); break;
            }
            Console.WriteLine("Jobs Waiting : ({0}) P0, ({1}) P1, ({2}) P2, ({3}) P3", fileAttenteP0.Count, fileAttenteP1.Count, fileAttenteP2.Count, fileAttenteP3.Count);
        }

        private Job Peek()
        {
            Job traitement = null;
            if (fileAttenteP0.Count != 0) { traitement = fileAttenteP0.Peek(); }
            else if (fileAttenteP1.Count != 0) { traitement = fileAttenteP1.Peek(); }
            else if (fileAttenteP2.Count != 0) { traitement = fileAttenteP2.Peek(); }
            else if (fileAttenteP3.Count != 0) { traitement = fileAttenteP3.Peek(); }
            return traitement;
        }


        private void Dequeue(Job traitement)
        {
            switch (traitement.priorite)
            {
                case 0: fileAttenteP0.Dequeue(); break;
                case 1: fileAttenteP1.Dequeue(); break;
                case 2: fileAttenteP2.Dequeue(); break;
                case 3: fileAttenteP3.Dequeue(); break;
            }
        }


        
        //-----------------------------------
        //
        // Lanceur
        //
        //-----------------------------------

        public void TryStartNext()
        {

            Job traitement = Peek();
            if (traitement != null && traitement.TryStart())
            {
                Dequeue(traitement);
            }
        }
    }
}

    




