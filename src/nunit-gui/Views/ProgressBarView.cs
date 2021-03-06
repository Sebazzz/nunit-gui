﻿// ***********************************************************************
// Copyright (c) 2016 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;
using System.Diagnostics;
using System.Windows.Forms;
using NUnit.Gui.Model;
using NUnit.UiKit.Controls;
using System.Collections.Generic;

namespace NUnit.Gui.Views
{
    // Interface used in testing the presenter
    public interface IProgressBarView : IView
    {
        TestProgressBarStatus Status { get; set; }

        void Initialize(int max);
        void AddStatus(TestStatus status);
    }

    public partial class ProgressBarView : UserControl, IProgressBarView
    {
        private int _maximum;
        private readonly Dictionary<TestStatus, int> _statusLabels;
        private readonly TestStatus[] _testStatuses;

        public ProgressBarView()
        {
            InitializeComponent();
            _statusLabels = new Dictionary<TestStatus, int>();

            var values = Enum.GetValues(typeof(TestStatus));
            _testStatuses = new TestStatus[values.Length];
            values.CopyTo(_testStatuses, 0);
        }

        public void Initialize(int max)
        {
            Debug.Assert(max > 0, "Maximum value must be > 0");

            _maximum = max;
            _progress = 0;
            _status = TestProgressBarStatus.Success;
            foreach (var statusLabelKey in _testStatuses)
                _statusLabels[statusLabelKey] = 0;
            var newStatus = BuildStatusesText();

            InvokeIfRequired(() =>
            {
                testStatusesLabel.Text = newStatus;
                testProgressBar.Maximum = _maximum;
                testProgressBar.Value = _progress;
                testProgressBar.Status = _status;
            });
        }

        private int _progress;

        private TestProgressBarStatus _status;
        public TestProgressBarStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
                InvokeIfRequired(() => { testProgressBar.Status = _status; });
            }
        }

        private void InvokeIfRequired(MethodInvoker _delegate)
        {
            if (testProgressBar.InvokeRequired)
                testProgressBar.BeginInvoke(_delegate);
            else
                _delegate();
        }

        public void AddStatus(TestStatus status)
        {
            _progress++;
            ++_statusLabels[status];
            var newStatus = BuildStatusesText();
            InvokeIfRequired(() => { testStatusesLabel.Text = newStatus; testProgressBar.Value = _progress; });
        }

        private string BuildStatusesText()
        {
            string[] asdf = new string[_testStatuses.Length];
            for (int i = 0; i < _statusLabels.Count; i++)
            {
                TestStatus t = (TestStatus)i;
                asdf[i] = $"{t}:{_statusLabels[t]}";
            }
            var newStatus = String.Join(" | ", asdf);
            return newStatus;
        }
    }
}
