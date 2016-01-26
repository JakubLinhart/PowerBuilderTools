@echo off

cd .\Enumerated
del *.cs -a
cd ..

cd .\GraphicObject
del *.cs -a
cd ..

cd .\NonVisualObject
del *.cs -a
	cd .\Throwable
	del *.cs -a
	cd ..
cd ..

cd .\PbToCPPObject
del *.cs -a
cd ..

cd .\Structure
del *.cs -a
cd ..

cd .\TraceActivityNode
del *.cs -a
cd ..

cd .\ValueTypes
del Any.cs
del Blob.cs
del Boolean.cs
del Date.cs
del DateTime.cs
del Decimal.cs
del Double.cs
del Integer.cs
del Long.cs
del Real.cs
del String.cs
del Time.cs
del UnsignedInteger.cs
del UnsignedLong.cs
cd ..
