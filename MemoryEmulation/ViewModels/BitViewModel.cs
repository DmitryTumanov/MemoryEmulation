using System.Windows.Media;
using MemoryEmulation.DataContext.Enums;
using MemoryEmulation.DataContext.Models;
using MemoryEmulation.DataContext.MVVMBasics;

namespace MemoryEmulation.ViewModels
{
    public class BitViewModel : BaseModel
    {
        private readonly int _bitIndex;

        public readonly Bit Bit;

        public BitViewModel(Bit bit, int index)
        {
            Bit = bit;
            _bitIndex = index;
        }

        public Brush Color
        {
            get
            {
                switch (Bit.State)
                {
                    case BitStates.Broken:
                        return new SolidColorBrush(Colors.Red);
                    case BitStates.One:
                        return new SolidColorBrush(Colors.DeepSkyBlue);
                    case BitStates.Zero:
                        return new SolidColorBrush(Colors.AliceBlue);
                }

                return new SolidColorBrush(Colors.Black);
            }
        }

        public string TooltipText
        {
            get
            {
                var state = string.Empty;
                switch (Bit.State)
                {
                    case BitStates.Broken:
                        state = nameof(BitStates.Broken);
                        break;
                    case BitStates.One:
                        state = nameof(BitStates.One);
                        break;
                    case BitStates.Zero:
                        state = nameof(BitStates.Zero);
                        break;
                }

                return $"bit#{_bitIndex}\nState: {state}";
            }
        }

        public bool IsBroken => Bit.State == BitStates.Broken;

        public void SetState(BitStates bitState)
        {
            SetProperty(ref Bit.State, bitState, nameof(Color));
        }

        public void SetNextState()
        {
            var intState = (int)Bit.State;
            var newState = (BitStates)(Bit.State == BitStates.One ? -1 : intState + 1);
            SetState(newState);
        }
    }
}
