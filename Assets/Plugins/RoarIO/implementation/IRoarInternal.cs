using System;
using System.Collections;

public interface IRoarInternal
{
	bool isDebug();
	void doCoroutine(IEnumerator methodName);
	IWebAPI WebAPI { get; }
}
