using GrabApp;
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



namespace GrabApp.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Penumpang penumpangRina = new Penumpang("Rina");

           
            Mitra mitraJoko = new Mitra("Pak Joko");

            
            GrabRide pesananRide = new GrabRide();
            penumpangRina.TampilkanLayanan(pesananRide);
           
            

            Console.ReadLine();
        }
    }
}