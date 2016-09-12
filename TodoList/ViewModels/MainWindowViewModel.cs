﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using TodoList.Helpers;
using TodoList.Helpers.EventAggregator;
using TodoList.Models;

namespace TodoList.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase, ISubscriber<TaskItemAdded>
    {
        private string _taskName;
        private DateTime _selectedDate;
        private IEventAggregator EventAggregator { get; set; }

        public ObservableCollection<TaskItem> OldTasks { get; set; }
        public ObservableCollection<TaskItem> CurrentTasks { get; set; }
        public ObservableCollection<TaskItem> FollowingTasks { get; set; }

        public string TaskName
        {
            get { return _taskName; }
            set
            {
                _taskName = value;
                NotifyPropertyChanged(nameof(TaskName));
            }
        }

        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                _selectedDate = value;
                NotifyPropertyChanged(nameof(SelectedDate));
            }
        }

        public MainWindowViewModel()
        {
            InitializeLists();
            InitializeProperties();
            InitializeEventAggregator();
            InitializeCommands();
        }

        private void InitializeProperties()
        {
            TaskName = string.Empty;
            SelectedDate = DateTime.Now;
        }

        private void InitializeEventAggregator()
        {
            EventAggregator = new SimpleEventAggregator();
            EventAggregator.Subscribe(this);
        }

        private void InitializeLists()
        {
            OldTasks = new ObservableCollection<TaskItem>();
            CurrentTasks = new ObservableCollection<TaskItem>();
            FollowingTasks = new ObservableCollection<TaskItem>();
            OldTasks.CollectionChanged += (sender, args) => NotifyPropertyChanged(nameof(OldTasks));
            CurrentTasks.CollectionChanged += (sender, args) => NotifyPropertyChanged(nameof(CurrentTasks));
            FollowingTasks.CollectionChanged += (sender, args) => NotifyPropertyChanged(nameof(FollowingTasks));
        }

        public void OnEvent(TaskItemAdded e)
        {
            PutEventToList(e.Task);
        }

        private void PutEventToList(TaskItem task)
        {
            if (task.DueToDate.Date == SelectedDate.Date)
                CurrentTasks.Add(task);
            else if (task.DueToDate.Date < SelectedDate.Date)
                OldTasks.Add(task);
            else if (IsInThisWeek(task.DueToDate))
                FollowingTasks.Add(task);
        }

        private bool IsInThisWeek(DateTime date)
        {
            int daysToLastDayOfWeek = 7 - (int)SelectedDate.DayOfWeek;
            DateTime endOfTheWeek = SelectedDate.AddDays(daysToLastDayOfWeek);
            if (date.Date > SelectedDate.Date && date.Date <= endOfTheWeek.Date)
                return true;
            return false;
        }
    }
}
