using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using RexSimulatorCLI.Panels;

namespace RexSimulatorCLI
{
    internal class PanelManager
    {
        private List<Panel> _panels;
        private int _activePanelIndex;

        public PanelManager()
        {
            _panels = new List<Panel>();
        }

        public Panel ActivePanel
        {
            get { return _panels[_activePanelIndex]; }
            set
            {
                if (_panels.Contains(value))
                    _activePanelIndex = _panels.IndexOf(value);
                else
                    throw new Exception(string.Format("Panel does not exist"));
            }
        }

        public void MoveToNextPanel()
        {
            if (_activePanelIndex == _panels.Count - 1)
            {
                _activePanelIndex = 0;
                ActivePanel.SendBufferToStdout();
            }
            else
            {
                _activePanelIndex++;
                ActivePanel.SendBufferToStdout();
            }
        }

        public void MoveToPrevPanel()
        {
            if (_activePanelIndex == 0)
            {
                _activePanelIndex = _panels.Count - 1;
                ActivePanel.SendBufferToStdout();
            }
            else
            {
                _activePanelIndex--;
                ActivePanel.SendBufferToStdout();
            }
        }

        public void AddPanel(string name, Panel panel)
        {
            panel.Name = name;
            _panels.Add(panel);
            if (_panels.Count == 1)
            {
                _activePanelIndex = 0;
            }
        }

        public void RemovePanel(string name)
        {
            var panel = _panels.Find(p => p.Name == name);
            if (panel == null)
                throw new Exception("Panel does not exist");

            if (_activePanelIndex == _panels.IndexOf(panel))
                _activePanelIndex = -1;
            _panels.Remove(panel);
        }

        public void SendInputToActivePanel(ConsoleKeyInfo info)
        {
            _panels[_activePanelIndex].SendInput(info);
        }

        public void SendInputToAllPanels(ConsoleKeyInfo info)
        {
            foreach (var panel in _panels)
            {
                panel.SendInput(info);
            }
        }
    }
}
