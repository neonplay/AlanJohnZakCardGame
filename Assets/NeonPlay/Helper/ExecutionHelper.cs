using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace NeonPlayHelper {

	public interface IExecutable {

		void Execute();
	}

	public class ExecutionQueue {

		private Queue<IExecutable> executionQueue = new Queue<IExecutable>();
		private HashSet<IExecutable> removedSet = new HashSet<IExecutable>();

		public void Enqueue(IExecutable execution) {

			if (executionQueue == null) {

				throw new InvalidOperationException("Execution Queue has been shutdown and will no longer process the queue");
			}

			lock (executionQueue) {

				executionQueue.Enqueue(execution);
			}
		}

		public void Remove(IExecutable execution) {

			lock (executionQueue) {

				removedSet.Add(execution);
			}
		}

		public void Process() {

			Queue<IExecutable> queue = null;
			HashSet<IExecutable> removed = null;

			lock (executionQueue) {

				if (executionQueue.Count > 0) {

					queue = executionQueue;
					removed = removedSet;
					executionQueue = new Queue<IExecutable>();
					removedSet = new HashSet<IExecutable>();
				}
			}

			if (queue != null) {

				while (queue.Count > 0) {

					IExecutable execution = queue.Dequeue();

					if (!removed.Contains(execution)) {

						try {

							execution.Execute();

						} catch (Exception exception) {

							Debug.LogException(exception);
						}
					}
				}
			}
		}

		public void Shutdown() {

			lock (executionQueue) {

				executionQueue = null;
			}
		}
	}

	public class ExecutionHelper : MonoBehaviour {

		private class ExecutableAction : IExecutable {

			public readonly Action Action;

			public ExecutableAction(Action executeAction) {

				Action = executeAction;
			}

			public void Execute() {

				Action();
			}
		}

		private class ExecutableAction<TValue> : IExecutable {

			public readonly TValue Value;
			public readonly Action<TValue> Action;

			public ExecutableAction(TValue value, Action<TValue> executeAction) {

				Value = value;
				Action = executeAction;
			}

			public void Execute() {

				Action(Value);
			}
		}

		private class ExecutableAction<TFirstValue, TSecondValue> : IExecutable {

			public readonly TFirstValue FirstValue;
			public readonly TSecondValue SecondValue;
			public readonly Action<TFirstValue, TSecondValue> Action;

			public ExecutableAction(TFirstValue firstValue, TSecondValue secondValue, Action<TFirstValue, TSecondValue> executeAction) {

				FirstValue = firstValue;
				SecondValue = secondValue;
				Action = executeAction;
			}

			public void Execute() {

				Action(FirstValue, SecondValue);
			}
		}

		private class ExecutableAction<TFirstValue, TSecondValue, TThirdValue> : IExecutable {

			public readonly TFirstValue FirstValue;
			public readonly TSecondValue SecondValue;
			public readonly TThirdValue ThirdValue;
			public readonly Action<TFirstValue, TSecondValue, TThirdValue> Action;

			public ExecutableAction(TFirstValue firstValue, TSecondValue secondValue, TThirdValue thirdValue, Action<TFirstValue, TSecondValue, TThirdValue> executeAction) {

				FirstValue = firstValue;
				SecondValue = secondValue;
				ThirdValue = thirdValue;
				Action = executeAction;
			}

			public void Execute() {

				Action(FirstValue, SecondValue, ThirdValue);
			}
		}

		private ExecutionQueue executionQueue;

		private static ExecutionHelper instance;
		private static readonly int MainThreadId = Thread.CurrentThread.ManagedThreadId;

		public static void Activate() {

			if (Thread.CurrentThread.ManagedThreadId != MainThreadId) {

				throw new InvalidOperationException("Activate can only be called from the main thread");
			}

			if (instance == null) {

				GameObject gameObject = new GameObject("ExecutionHelper");

				instance = gameObject.AddComponent<ExecutionHelper>();
				DontDestroyOnLoad(gameObject);
			}
		}

		protected virtual void Awake() {

			executionQueue = new ExecutionQueue();
		}

		protected void Update() {

			executionQueue.Process();
		}

		protected virtual void OnDestroy() {

			executionQueue.Shutdown();
		}

		public static void EnqueueExecution(IExecutable execution) {

			if (execution != null) {

				instance.executionQueue.Enqueue(execution);
			}
		}

		public static void EnqueueExecution(Action executionAction) {

			if (executionAction != null) {

				instance.executionQueue.Enqueue(new ExecutableAction(executionAction));
			}
		}

		public static void EnqueueExecution<TValue>(TValue value, Action<TValue> executionAction) {

			if (executionAction != null) {

				instance.executionQueue.Enqueue(new ExecutableAction<TValue>(value, executionAction));
			}
		}

		public static void EnqueueExecution<TFirstValue, TSecondValue>(TFirstValue firstValue, TSecondValue secondValue, Action<TFirstValue, TSecondValue> executionAction) {

			if (executionAction != null) {

				instance.executionQueue.Enqueue(new ExecutableAction<TFirstValue, TSecondValue>(firstValue, secondValue, executionAction));
			}
		}

		public static void EnqueueExecution<TFirstValue, TSecondValue, TThirdValue>(TFirstValue firstValue, TSecondValue secondValue, TThirdValue thirdValue, Action<TFirstValue, TSecondValue, TThirdValue> executionAction) {

			if (executionAction != null) {

				instance.executionQueue.Enqueue(new ExecutableAction<TFirstValue, TSecondValue, TThirdValue>(firstValue, secondValue, thirdValue, executionAction));
			}
		}

		public static void RemoveExecution(IExecutable execution) {

			instance.executionQueue.Remove(execution);
		}
	}
}
