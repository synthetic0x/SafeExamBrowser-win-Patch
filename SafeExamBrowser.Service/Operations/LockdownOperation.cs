/*
 * Copyright (c) 2026 ETH Zürich, IT Services
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using System;
using System.Collections.Generic;
using SafeExamBrowser.Core.Contracts.OperationModel;
using SafeExamBrowser.Lockdown.Contracts;
using SafeExamBrowser.Logging.Contracts;

namespace SafeExamBrowser.Service.Operations
{
	internal class LockdownOperation : SessionOperation
	{
		private readonly IFeatureConfigurationBackup backup;
		private readonly IFeatureConfigurationMonitor monitor;
		private readonly ILogger logger;

		public LockdownOperation(
			IFeatureConfigurationBackup backup,
			IFeatureConfigurationFactory factory,
			IFeatureConfigurationMonitor monitor,
			ILogger logger,
			SessionContext sessionContext) : base(sessionContext)
		{
			this.backup = backup;
			this.monitor = monitor;
			this.logger = logger;
		}

		public override OperationResult Perform()
		{
			logger.Info("Skipping lockdown [patched].");

			return OperationResult.Success;
		}

		public override OperationResult Revert()
		{
			logger.Info("Skipping lockdown reversion [patched].");

			return OperationResult.Success;
		}

		private bool TryRestore(IFeatureConfiguration configuration)
		{
			var success = configuration.Restore();

			if (success)
			{
				backup.Delete(configuration);
			}
			else
			{
				logger.Error($"Failed to restore {configuration}!");
			}

			return success;
		}

		private bool TrySet(IFeatureConfiguration configuration, bool disable)
		{
			var success = false;
			var status = FeatureConfigurationStatus.Undefined;

			configuration.Initialize();
			backup.Save(configuration);

			if (disable)
			{
				success = configuration.DisableFeature();
				status = FeatureConfigurationStatus.Disabled;
			}
			else
			{
				success = configuration.EnableFeature();
				status = FeatureConfigurationStatus.Enabled;
			}

			if (success)
			{
				monitor.Observe(configuration, status);
			}
			else
			{
				logger.Error($"Failed to configure {configuration}!");
			}

			return success;
		}
	}
}
