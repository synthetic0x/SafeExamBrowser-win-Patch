/*
 * Copyright (c) 2026 ETH Zürich, IT Services
 * 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using System;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Monitoring.Contracts.Mouse;
using SafeExamBrowser.Settings.Monitoring;
using SafeExamBrowser.WindowsApi.Contracts;
using SafeExamBrowser.WindowsApi.Contracts.Events;

namespace SafeExamBrowser.Monitoring.Mouse
{
	public class MouseInterceptor : IMouseInterceptor
	{
		private Guid? hookId;
		private INativeMethods nativeMethods;

		public MouseInterceptor(ILogger logger, INativeMethods nativeMethods, MouseSettings settings)
		{
			this.nativeMethods = nativeMethods;
		}

		public void Start()
		{
			hookId = nativeMethods.RegisterMouseHook(MouseHookCallback);
		}

		public void Stop()
		{
			if (hookId.HasValue)
			{
				nativeMethods.DeregisterMouseHook(hookId.Value);
			}
		}

		private bool MouseHookCallback(MouseButton button, MouseButtonState state, MouseInformation info)
		{
			return false;
		}
	}
}
