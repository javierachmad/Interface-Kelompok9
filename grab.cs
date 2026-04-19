using GrabApp;
using GrabApp.Core;
using System;

namespace GrabApp.Core
{
    public interface IGrabPayment
    {
        void ProsesPembayaran(decimal jumlah);
    }

    public abstract class GrabService : IGrabPayment
    {
        public string NamaLayanan { get; set; }

        public GrabService(string namaLayanan)
        {
            NamaLayanan = namaLayanan;
        }

        public void TampilkanLayanan()
        {
            Console.WriteLine($"Layanan: {NamaLayanan}");
        }

        public abstract decimal Tarif(int jumlah);

        public void ProsesPembayaran(decimal jumlah)
        {
            Console.WriteLine($"Memproses pembayaran sebesar {jumlah} untuk layanan {NamaLayanan}");
        }
    }

    public class GrabRide : GrabService
    {
        public GrabRide() : base("Grab Ride")
        {

        }

        public override decimal Tarif(int jumlah)
        {
            return jumlah * 5000;
        }

    }

    public class GrabCar : GrabService
    {
        public GrabCar() : base("Grab Car")
        {
        }
        public override decimal Tarif(int jumlah)
        {
            return jumlah * 10000;
        }
    }

    public class Mitra
    {
        public string NamaMitra { get; set; }

        public Mitra(string nama)
        {
            NamaMitra = nama;
        }

        public void TerimaOrder(string namaPenumpang)
        {
            Console.WriteLine($"[Mitra] {NamaMitra} sedang meluncur ke titik penjemputan {namaPenumpang}.");
        }
    }

    public class Penumpang
    {
        public string NamaPenumpang { get; set; }

        public Penumpang(string nama)
        {
            NamaPenumpang = nama;
        }

        public void PesanPerjalanan(Mitra driver, GrabService layanan, int jarak)
        {
            Console.WriteLine($"\n--- {NamaPenumpang} Membuat Pesanan ---");

            // Menggunakan objek layanan
            layanan.TampilkanLayanan();
            decimal totalTarif = layanan.Tarif(jarak);

            // Menggunakan objek Mitra
            driver.TerimaOrder(NamaPenumpang);

            // Mengeksekusi antarmuka pembayaran
            layanan.ProsesPembayaran(totalTarif);
        }
    }

}

namespace GrabApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Selamat Datang di GrabApp ===\n");

            
            Mitra driver1 = new Mitra("Budi Santoso");
            Mitra driver2 = new Mitra("Andi Wijaya");

            
            GrabRide grabRide = new GrabRide();
            GrabCar grabCar = new GrabCar();

            
            Penumpang penumpang1 = new Penumpang("Siti Rahayu");
            Penumpang penumpang2 = new Penumpang("Rudi Hartono");

            penumpang1.PesanPerjalanan(driver1, grabRide, 5);

            
            penumpang2.PesanPerjalanan(driver2, grabCar, 8);

            
            Console.WriteLine("\n--- Info Tarif ---");
            Console.WriteLine($"Tarif GrabRide per 1 km : Rp {grabRide.Tarif(1):N0}");
            Console.WriteLine($"Tarif GrabCar per 1 km  : Rp {grabCar.Tarif(1):N0}");

            
            Console.WriteLine("\n--- Pembayaran via Interface ---");
            IGrabPayment pembayaran = new GrabRide();
            pembayaran.ProsesPembayaran(25000);

            Console.WriteLine("\n=== Terima kasih telah menggunakan GrabApp ===");
        }
    }
}