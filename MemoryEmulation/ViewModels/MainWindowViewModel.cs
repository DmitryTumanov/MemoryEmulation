using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using MemoryEmulation.BusinessLogic.Generators;
using MemoryEmulation.BusinessLogic.MemoryRepairers;
using MemoryEmulation.BusinessLogic.MemoryRepairers.Implementations;
using MemoryEmulation.DataContext.Enums;
using MemoryEmulation.DataContext.Models;
using MemoryEmulation.DataContext.MVVMBasics;
using MemoryEmulation.Helpers;
using Microsoft.Win32;

namespace MemoryEmulation.ViewModels
{
    public class MainWindowViewModel : BaseModel
    {
        #region [Fields]

        private List<IMemoryRepairer> _memoryRepairers;
        private VmCollection<BitViewModel> _memory;
        private Dispatcher _dispatcher;
        private long _bitesCount;
        private bool _isBusy;
        private int _bitIndex;
        private int _repairerIndex;

        #endregion

        #region [Comands]

        public ICommand GenerateMemory { get; set; }
        public ICommand BrokeMemory { get; set; }
        public ICommand RepairMemory { get; set; }
        public ICommand BitStateChange { get; set; }
        public ICommand OpenFile { get; set; }

        #endregion

        #region [Constructor]

        public MainWindowViewModel(Dispatcher dispatcher)
        {
            InitializeProperties(dispatcher);
            InitializeCommands();
        }

        #endregion

        #region [Properties]

        public VmCollection<BitViewModel> Memory => _memory;

        public List<IMemoryRepairer> MemoryRepairers => _memoryRepairers;

        public long BitesCount
        {
            get => _bitesCount;
            set => SetProperty(ref _bitesCount, value, nameof(BitesCount));
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value, nameof(IsBusy));
        }

        public int BitIndex
        {
            get => _bitIndex;
            set => SetProperty(ref _bitIndex, value, nameof(BitIndex));
        }

        public int RepairerIndex
        {
            get => _repairerIndex;
            set => SetProperty(ref _repairerIndex, value, nameof(RepairerIndex));
        }

        #endregion

        #region [Help Methods]

        private void InitializeProperties(Dispatcher dispatcher)
        {
            _memory = new VmCollection<BitViewModel>(dispatcher);
            _dispatcher = dispatcher;
            _bitesCount = 100;
            _memoryRepairers = new List<IMemoryRepairer>
            {
                new RandomMemoryRepairer(),
                new RandomWithPositionSavingMemoryRepairer()
            };
        }

        private void InitializeCommands()
        {
            GenerateMemory = new Command(GenerateMemoryBits);
            BrokeMemory = new Command(BrokeMemoryBits);
            RepairMemory = new Command(RepairMemoryBits);
            BitStateChange = new Command(BitStateChangeEvent);
            OpenFile = new Command(OpenFileAndConvertToBits);
        }

        private void BitStateChangeEvent(object _)
        {
            var neededBit = Memory[BitIndex];
            neededBit.SetNextState();
            Memory.FireChange(neededBit);
        }

        private void GenerateMemoryBits(object _)
        {
            Memory.Clear();
            RefreshMemoryRepairers();
            var memoryGenerator = MemoryGenerator.CreateGenerator(BitesCount);
            StartTask(() =>
            {
                GenerateMemoryFromEnumerable(memoryGenerator);
            });
        }

        private void BrokeMemoryBits(object _)
        {
            if (BitesCount > Memory.Count)
            {
                return;
            }

            StartTask(() =>
            {
                var brokenBits = MemoryHelper.GetRandomBits(Memory.Where(x => !x.IsBroken), (int) BitesCount);
                foreach (var brokenBit in brokenBits)
                {
                    brokenBit.SetState(BitStates.Broken);
                    Memory.FireChange(brokenBit);
                }
            });
        }

        private void RepairMemoryBits(object _)
        {
            if (BitesCount > Memory.Count)
            {
                return;
            }

            StartTask(() =>
            {
                var memoryRepairer = MemoryRepairers[RepairerIndex];
                var brokenBitsIndexes = memoryRepairer.GetBrokenBitsIndexes(Memory.Select(x=>x.Bit), (int) BitesCount);
                foreach (var brokenBitsIndex in brokenBitsIndexes)
                {
                    var brokenBit = Memory[brokenBitsIndex];
                    brokenBit.SetState(BitStates.Zero);
                    Memory.FireChange(brokenBit);
                }
            });
        }

        private void OpenFileAndConvertToBits(object _)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                using (var stream = openFileDialog.OpenFile())
                {
                    var bitArray = MemoryGenerator.GetBiteArrayFromStream(stream);
                    Memory.Clear();
                    StartTask(() =>
                    {
                        GenerateMemoryFromEnumerable(bitArray);
                    });
                }
            }
        }

        private static Task StartTask(Action action)
        {
            return new TaskFactory().StartNew(action);
        }

        private void ContinueTask(Task task, Action action)
        {
            task.ContinueWith(t =>
            {
                _dispatcher.Invoke(action);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void RefreshMemoryRepairers()
        {
            foreach (var memoryRepairer in MemoryRepairers)
            {
                memoryRepairer.Refresh();
            }
        }

        private void GenerateMemoryFromEnumerable(IEnumerable<Bit> bits)
        {
            var currentIndex = 0;
            var memory = bits.Select(bit => new BitViewModel(bit, currentIndex++)).ToList();
            Memory.AddRange(memory);
        }

        #endregion
    }
}
