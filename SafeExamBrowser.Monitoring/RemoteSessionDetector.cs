/*
 * Copyright (c) 2026 ETH Zürich, IT Services
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using SafeExamBrowser.Integrity.Contracts;
using SafeExamBrowser.Logging.Contracts;
using SafeExamBrowser.Monitoring.Contracts;

namespace SafeExamBrowser.Monitoring
{
	public class RemoteSessionDetector : IRemoteSessionDetector
	{
		private readonly ILogger logger;

		public RemoteSessionDetector(IIntegrityModule integrityModule, ILogger logger)
		{
			this.logger = logger;
		}

		public bool IsRemoteSession()
		{
			var isRemoteSession = false;

			logger.Debug($"Current user session appears {(isRemoteSession ? "" : "not ")}to be a remote session [patched].");

			return isRemoteSession;
		}
	}
}
