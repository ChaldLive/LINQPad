<Query Kind="Program">
  <Connection>
    <ID>d2189dc6-eff4-424c-ac05-8dd6992511c0</ID>
    <Persist>true</Persist>
    <Driver>LinqToSql</Driver>
    <Server>fac-usql01.facilia.dk\facilia2014,1435</Server>
    <CustomAssemblyPath>C:\Facilia\Main\Facilia\Facilia.Data\bin\Debug\Facilia.Data.dll</CustomAssemblyPath>
    <CustomTypeName>Facilia.Data.Entiteter.DatabaseContext</CustomTypeName>
    <Database>FaciliaDatabase</Database>
  </Connection>
</Query>

void Main()
{
	
	PeriodeTjek p = new PeriodeTjek(21,"2508551058");
	
	Akasse_JobCenterPeriode jpc = p.HentSeniorAbsensePeriode(this).FirstOrDefault();
	IEnumerable<Akasse_Timebank> timeBanker = p.HentOverlappendeTimebankPerioder(this,"2508551058").OrderBy(tb=>tb.PeriodeStartDato);
	
	jpc.Dump();
	timeBanker.Dump();
	
}

public class PeriodeTjek
{
	private int _akasseNr;
	private string _cprNr;
	
	public PeriodeTjek(int akasseNr, string cprNr)
	{
		_akasseNr = akasseNr;
		_cprNr = cprNr;
	}
	
	#region Properties

	public int AkasseNr
	{
		get { return _akasseNr; }
		set {_akasseNr = value;}
	}
	public string CprNr
	{
		get { return _cprNr; }
		set { _cprNr = value; }
	}
	#endregion
	
	public IEnumerable<Akasse_JobCenterPeriode> HentSeniorAbsensePeriode(UserQuery db)
	{
		Akasse_AkasseMedlemsskab medlemskab = FindMedlemskab(db, CprNr);
		return medlemskab.Akasse_JobCenterPeriode.Where(jcp=>!string.IsNullOrEmpty(jcp.AbsenceTypeIdentifier) && jcp.AbsenceTypeIdentifier.Equals("25",StringComparison.CurrentCultureIgnoreCase));
	}
	
	
	public IEnumerable<Akasse_Timebank> HentOverlappendeTimebankPerioder(UserQuery db, string cprNr)
	{
		Akasse_AkasseMedlemsskab medlemskab = FindMedlemskab(db, CprNr);
		var overlapTimebanker = from timeBank in medlemskab.Akasse_Timebank
								from jcp in medlemskab.Akasse_JobCenterPeriode
								where !string.IsNullOrEmpty(jcp.AbsenceTypeIdentifier)
								&& jcp.AbsenceTypeIdentifier.Equals("25")
								&& timeBank.PeriodeStartDato >= jcp.Startdato
								&& timeBank.PeriodeSlutDato <= jcp.Slutdato
								select timeBank;
								
		return overlapTimebanker;
	}


	public void TestTimebankSelection(UserQuery db)
	{
		Akasse_AkasseMedlemsskab medlemskab = FindMedlemskab(db, CprNr);
		var overlapTimebanker = from timeBank in medlemskab.Akasse_Timebank
								from jcp in medlemskab.Akasse_JobCenterPeriode
								where !string.IsNullOrEmpty(jcp.AbsenceTypeIdentifier) 
								&& jcp.AbsenceTypeIdentifier.Equals("25")
								&& timeBank.PeriodeStartDato >= jcp.Startdato 
								&& timeBank.PeriodeSlutDato <= jcp.Slutdato						
								select timeBank;
								
								
		var resultatTimebanker = medlemskab.Akasse_Timebank.Except(overlapTimebanker);
		resultatTimebanker.Dump();
	}
	
	public IEnumerable<Akasse_Timebank> TestPeriodeOverlapITimebank(UserQuery db)
	{
		IEnumerable<Akasse_Timebank> result = null;
		Akasse_AkasseMedlemsskab medlemskab = FindMedlemskab(db,CprNr);
		int countOfTimebankeInstanser = medlemskab.Akasse_Timebank.Count;
		medlemskab.Akasse_Timebank.Dump();
		IEnumerable<Akasse_Timebank> res1 = from timeBank in medlemskab.Akasse_Timebank
											from jcp in medlemskab.Akasse_JobCenterPeriode
											where timeBank.PeriodeStartDato >= jcp.Startdato
											&& (timeBank.PeriodeSlutDato != null && jcp.Slutdato != null)
											&& (timeBank.PeriodeSlutDato <= jcp.Slutdato) &&
												  jcp.AbsenceTypeIdentifier.Equals("25")
											select timeBank;
											
											
											
		res1.Dump();
		result = medlemskab.Akasse_Timebank.Except(res1);
		result.Dump();
		return result;
	}



	public Akasse_AkasseMedlemsskab HentMedlemskab(UserQuery db, string cprNummer)
	{
		Akasse_AkasseMedlemsskab result = null;

		result = (from person in db.Akasse_Person
				 join medlem in db.Akasse_Medlem on person.Person_Uid equals medlem.Person_Medlem_Ref
				 join akasse in db.Akasse_Akasse on medlem.Akasse_Medlem_Ref equals akasse.Akasse_Uid
				 join medlemskab in db.Akasse_AkasseMedlemsskab on medlem.Medlem_Uid equals medlemskab.Medlem_AkasseMedlemsskaber_Ref
				 where person.Cprnr == cprNummer
				 select medlemskab).FirstOrDefault();
		
		return result;
	}
	
	
	public Akasse_AkasseMedlemsskab FindMedlemskab(UserQuery db, string cprNr)
	{
		return db.Akasse_AkasseMedlemsskab.OrderByDescending(m=>m.Indmeldelsesdato).Where(mdl=>mdl.Akasse_Medlem.Akasse_Person.Cprnr == cprNr).FirstOrDefault();
	}
	
}