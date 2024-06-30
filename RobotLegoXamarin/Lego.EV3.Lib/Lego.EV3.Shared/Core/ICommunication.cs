﻿using System;
using System.Threading.Tasks;
#if WINRT
using Windows.Foundation;
#endif

namespace Lego.Ev3.Core
{
	/// <summary>
	/// Interface for communicating with the EV3 brick
	/// </summary>
	public interface ICommunication
	{
		/// <summary>
		/// Fired when a full report is ready to parse and process.
		/// </summary>
		event EventHandler<ReportReceivedEventArgs> ReportReceived;

		/// <summary>
		/// Connect to the EV3 brick.
		/// </summary>
#if WINRT
		IAsyncAction
#else
		Task
#endif
		ConnectAsync(string deviceName = null);

		/// <summary>
		/// Disconnect from the EV3 brick.
		/// </summary>
		void Disconnect();

		/// <summary>
		/// Write a report to the EV3 brick.
		/// </summary>
		/// <param name="data"></param>
#if WINRT
		IAsyncAction
#else
		Task
#endif
		WriteAsync(byte[] data);
	}
}
