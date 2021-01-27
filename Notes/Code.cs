                Uri uriToAppx = new Uri(GetResourcePath("SimpleApp_1.0.0.0_x64.msixbundle"));
                List<Uri> urisToDependencies = GetDependencies();

                //InstallCertificate("SimpleApp_1.0.0.0_x64.cer");

                var pkgManager = new PackageManager();

                var Action = pkgManager.AddPackageAsync(uriToAppx, urisToDependencies, DeploymentOptions.ForceTargetApplicationShutdown);

                //AsyncOperationProgressHandler<DeploymentResult, DeploymentProgress> asyncProgress = null;
                //AsyncOperationWithProgressCompletedHandler<DeploymentResult, DeploymentProgress> asyncCompleted = null;

                //asyncProgress += (IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> result, DeploymentProgress deployprogress) =>
                //{
                //    var appxProgress = new AppxProgress(deployprogress.percentage, "...");
                //    progress.Report(appxProgress);
                //};

                //asyncCompleted += (op, result) =>
                //{
                //    var appxProgress = new AppxProgress(100, op.GetResults().IsRegistered ? "yes" : "no");
                //    progress.Report(appxProgress);
                //};

                //Action.Progress += asyncProgress;
                //Action.Completed += asyncCompleted;

                Action.Progress += (IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> result, DeploymentProgress deployprogress) =>
                {
                    var appxProgress = new AppxProgress(deployprogress.percentage, "...");
                    progress.Report(appxProgress);
                };

                Action.Completed += (op, result) =>
                {
                    var appxProgress = new AppxProgress(100, op.GetResults().IsRegistered ? "yes" : "no");
                    progress.Report(appxProgress);
                };

                await Action.AsTask(stop);

                //Action.Progress -= asyncProgress;
                //Action.Completed -= asyncCompleted;

                return true;
