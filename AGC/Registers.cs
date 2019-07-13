using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGC
{
    class Registers
    {
        private Dictionary<string, short> Address;
        public Registers()
        {
            Address = new Dictionary<string, short>()
            {
                {"A",           0b0000000000000000},
                {"L",           0b0000000000000000},
                {"Q",           0b0000000000000000},
                {"EBANK",       0b0000000000000000},
                {"FBANK",       0b0000000000000000},
                {"Z",           0b0000000000000000},
                {"BBANK",       0b0000000000000000},
                {"ARUPT",       0b0000000000000000},
                {"LRUPT",       0b0000000000000000},
                {"QRUPT",       0b0000000000000000},
                {"SAMPTIME",    0b0000000000000000},
                {"ZRUPT",       0b0000000000000000},
                {"BANKRUPT",    0b0000000000000000},
                {"BRUPT",       0b0000000000000000},
                {"CYR",         0b0000000000000000},
                {"SR",          0b0000000000000000},
                {"CYL",         0b0000000000000000},
                {"EDOP",        0b0000000000000000},
                {"TIME2",       0b0000000000000000},
                {"TIME1",       0b0000000000000000},
                {"TIME3",       0b0000000000000000},
                {"TIME4",       0b0000000000000000},
                {"TIME5",       0b0000000000000000},
                {"CDUX",        0b0000000000000000},
                {"CDUY",        0b0000000000000000},
                {"CDUT",        0b0000000000000000},
                {"OPTY",        0b0000000000000000},
                {"CDUS",        0b0000000000000000},
                {"OPTX",        0b0000000000000000},
                {"PIPAX",       0b0000000000000000},
                {"PIPAY",       0b0000000000000000},
                {"PIPAZ",       0b0000000000000000},
                {"BMAGX",       0b0000000000000000},
                {"BMAGY",       0b0000000000000000},
                {"BMAGZ",       0b0000000000000000},
                {"INLINK",      0b0000000000000000},
                {"RNRAD",       0b0000000000000000},
                {"GYROCTR",     0b0000000000000000},
                {"GYROCMD",     0b0000000000000000},
                {"CDUXCMD",     0b0000000000000000},
                {"CDUYCMD",     0b0000000000000000},
                {"CDUZCMD",     0b0000000000000000},
                {"CDUTMCD",     0b0000000000000000},
                {"OPTYCMD",     0b0000000000000000},
                {"TVCYAW",      0b0000000000000000},
                {"CDUSCMD",     0b0000000000000000},
                {"TVCPITCH",    0b0000000000000000},
                {"OPTXCMD",     0b0000000000000000},
                {"EMSD",        0b0000000000000000},
                {"THRUST",      0b0000000000000000},
                {"LEMONM",      0b0000000000000000},
                {"LOCALARM",    0b0000000000000000},
                {"BANKALRM",    0b0000000000000000},
                {"LVSQUARE",    0b0000000000000000},
                {"LV",          0b0000000000000000},
                {"X1",          0b0000000000000000},
                {"X2",          0b0000000000000000},
                {"S1",          0b0000000000000000},
                {"S2",          0b0000000000000000},
                {"QRET",        0b0000000000000000}
            };
        }

        public void writeRegister(string register, short value)
        {
            Address[register] = value;
        }

        public short getRegister(string register)
        {
            return Address[register];
        }

        public void moveRegister(string from, string to)
        {
            Address[to] = Address[from];
            clearRegister(from);
        }

        public void copyRegister(string from, string to)
        {
            Address[to] = Address[from];
        }

        public void clearRegister(string register)
        {
            Address[register] = 0b0000000000000000;
        }

        public void clearAllRegisters()
        {
            foreach (KeyValuePair<string, short> kvp in Address)
            {
                clearRegister(kvp.Key);
            }
        }

        public void printRegister(string register)
        {
            Console.WriteLine("{0}: {1}", register, Address[register]);
        }

        public void printRegisters()
        {
            foreach (KeyValuePair<string, short> kvp in Address)
            {
                Console.WriteLine("{0}: {1}", kvp.Key, kvp.Value);
            }
        }
    }
}
