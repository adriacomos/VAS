using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;


namespace VAS
{

    public class ParamsCreateScreenVIZ 
    {


        public virtual IPAddress AnimationAddress
        {
            get;
            set;
        }

        public virtual int AnimationPort
        {
            get;
            set;
        }

        public virtual IPAddress ValueAddress
        {
            get;
            set;
        }

        public virtual int ValuePort
        {
            get;
            set;
        }



        public ParamsCreateScreenVIZ(
            IPAddress animationAddress, int animationPort,
            IPAddress valueAddress, int valuePort)
        {
            AnimationAddress = animationAddress;
            AnimationPort = animationPort;
            ValueAddress = valueAddress;
            ValuePort = valuePort;
        }

    }
}
