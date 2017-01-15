using System;
using Windows.Devices.Gpio;

namespace HC_SR04_DistanceSensor
{
    public interface IHCSR04
    {
        bool ArePinsInitialized { get; }
        Guid Id { get; }
        double MeasuredDistance { get; }
        string Name { get; }
        GpioPin Pin_echo { get; }
        GpioPin Pin_trig { get; }
        double StandardDistance { get; }

        void AddMesure();
        void AddMesure(double mesureValue);
        double GetDistance();
        void InitializePins(int pin_trig, int pin_echo);
        void StartMeasuring();
        void StartSearchingStandardDistance();

        SensorMesuringSetting SensorMesuringSetting { get; }
    }
}