using UnityEngine;
using System;
using System.Collections.Generic;
using WibeSoft.Core.Singleton;

namespace WibeSoft.Core.Managers
{
    /// <summary>
    /// Manages centralized logging functionality for the game
    /// </summary>
    public class LogManager : Singleton<LogManager>
    {
        private const int MAX_LOG_HISTORY = 1000;
        private readonly Queue<LogEntry> _logHistory = new Queue<LogEntry>();
        private bool _isLoggingEnabled = true;
        private LogLevel _minimumLogLevel = LogLevel.Info;

        public void Log(string message, LogLevel level = LogLevel.Info, string category = "General")
        {
            if (!_isLoggingEnabled || level < _minimumLogLevel)
                return;

            var logEntry = new LogEntry
            {
                Message = message,
                Level = level,
                Category = category,
                Timestamp = DateTime.Now
            };

            // Add to history
            _logHistory.Enqueue(logEntry);
            if (_logHistory.Count > MAX_LOG_HISTORY)
            {
                _logHistory.Dequeue();
            }

            // Output to Unity console
            switch (level)
            {
                case LogLevel.Error:
                    Debug.LogError($"[{category}] {message}");
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning($"[{category}] {message}");
                    break;
                default:
                    Debug.Log($"[{category}] {message}");
                    break;
            }
        }

        public void LogInfo(string message, string category = "General")
        {
            Log(message, LogLevel.Info, category);
        }

        public void LogWarning(string message, string category = "General")
        {
            Log(message, LogLevel.Warning, category);
        }

        public void LogError(string message, string category = "General")
        {
            Log(message, LogLevel.Error, category);
        }

        public void SetMinimumLogLevel(LogLevel level)
        {
            _minimumLogLevel = level;
            Log($"Minimum log level set to: {level}", LogLevel.Info, "LogManager");
        }

        public void EnableLogging()
        {
            _isLoggingEnabled = true;
            Log("Logging enabled", LogLevel.Info, "LogManager");
        }

        public void DisableLogging()
        {
            Log("Logging disabled", LogLevel.Info, "LogManager");
            _isLoggingEnabled = false;
        }

        public void ClearHistory()
        {
            _logHistory.Clear();
            Log("Log history cleared", LogLevel.Info, "LogManager");
        }

        public LogEntry[] GetLogHistory()
        {
            return _logHistory.ToArray();
        }

        public LogEntry[] GetLogHistoryByLevel(LogLevel level)
        {
            return Array.FindAll(_logHistory.ToArray(), entry => entry.Level == level);
        }

        public LogEntry[] GetLogHistoryByCategory(string category)
        {
            return Array.FindAll(_logHistory.ToArray(), entry => entry.Category == category);
        }
    }

    public enum LogLevel
    {
        Info = 0,
        Warning = 1,
        Error = 2
    }

    public class LogEntry
    {
        public string Message { get; set; }
        public LogLevel Level { get; set; }
        public string Category { get; set; }
        public DateTime Timestamp { get; set; }
    }
} 