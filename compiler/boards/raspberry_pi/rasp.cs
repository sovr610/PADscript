using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Text;

namespace PADscript.boards.raspberry_pi
{
    public class rasp
    {
        private static GpioController controller = new GpioController(PinNumberingScheme.Logical);
        private static boardEnum currentBoard;
        public static void setSBCboard(string type)
        {
            if (type == "raspberrypi")
            {
                currentBoard = boardEnum.Raspberrypi;
            }
        }

        public static string[] getSBCboards()
        {
            string[] boards = new string[]
            {
                "raspberrypi"
            };
            return boards;
        }

        public static void setPinOutput(int pin)
        {
            controller.OpenPin(pin, PinMode.Output);
        }

        public static void setPinInput(int pin)
        {
            controller.OpenPin(pin, PinMode.Input);
        }

        public static void setPinInputPullDown(int pin)
        {
            controller.OpenPin(pin, PinMode.InputPullDown);
        }

        public static void setPinInputPullUp(int pin)
        {
            controller.OpenPin(pin, PinMode.InputPullUp);
        }

        public static void closePin(int pin)
        {
            controller.ClosePin(pin);
        }

        public static bool isPinHigh(int pin)
        {
            if (controller.Read(pin) == PinValue.High)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool isPinLow(int pin)
        {
            if (controller.Read(pin) == PinValue.Low)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void setPinHigh(int pin)
        {
            controller.Write(pin, PinValue.High);
        }

        public static void setPinLow(int pin)
        {
            controller.Write(pin, PinValue.Low);
        }

        public Script loadScript(Script l)
        {
            l.addFunc("setSBCboard", this, false);
            l.addFunc("getSBCboards", this, false);
            l.addFunc("setPinOutput", this, false);
            l.addFunc("setPinInput", this, false);
            l.addFunc("setPinInputPullDown", this, false);
            l.addFunc("setPinInputPullUp", this, false);
            l.addFunc("closePin", this, false);
            l.addFunc("isPinHigh", this, false);
            l.addFunc("isPinLow", this, false);
            l.addFunc("setPinHigh", this, false);
            l.addFunc("setPinLow", this, false);
            return l;
        }
    }
}
