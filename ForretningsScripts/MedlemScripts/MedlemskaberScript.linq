<Query Kind="Program">
  <Connection>
    <ID>eb26919d-36d7-4e88-a535-45e91cba10b6</ID>
    <Persist>true</Persist>
    <Driver>LinqToSql</Driver>
    <Server>factsql01.faciliatestservice.dk\TEST,1435</Server>
    <CustomAssemblyPath>C:\Facilia\Main\Facilia\Facilia.Data\bin\Debug\Facilia.Data.dll</CustomAssemblyPath>
    <CustomTypeName>Facilia.Data.Entiteter.DatabaseContext</CustomTypeName>
    <SqlSecurity>true</SqlSecurity>
    <Database>FaciliaDatabase_TD</Database>
    <UserName>sa_FaciliaWeb</UserName>
    <Password>AQAAANCMnd8BFdERjHoAwE/Cl+sBAAAAoFcircAn+EWQdSNol5HEVgAAAAACAAAAAAADZgAAwAAAABAAAABk9pdIs87g53z8r0mBV9WZAAAAAASAAACgAAAAEAAAAJvbCBMegyUpemOjvHM3Uz4QAAAAOsc3jfdvrZpwoYaoscMM4xQAAAD/GXP18eiVL+wgbJzoJ21n3/CJPQ==</Password>
  </Connection>
  <Output>DataGrids</Output>
</Query>

void Main()
{
	AkasseMedlemskab ms = new AkasseMedlemskab(15,"1604640497");
	var alleMedlemskaber = ms.AlleMedlemskaber(this);
	alleMedlemskaber.Dump();
}

public class AkasseMedlemskab
{
	#region Private fields
	private int _akasseNr;
	private string _cprNr;
	#endregion
	
	#region Constructor
	public AkasseMedlemskab(int akasseNr, string cprNr)
	{
		_akasseNr = akasseNr;
		_cprNr = cprNr;
	}
	#endregion
	
	#region Public propertis
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

	public IEnumerable<Akasse_AkasseMedlemsskab> AlleMedlemskaber(UserQuery p)
	{
		return p.Akasse_AkasseMedlemsskab.Where(m => (((m.Akasse_Medlem.Akasse_Person.Cprnr == CprNr) && (m.Akasse_Medlem.Akasse_Akasse.AkasseNr == AkasseNr))));
	}

	public IEnumerable<Akasse_AkasseMedlemsskab> AlleOphoerteMedlemskaber(UserQuery p)
	{
		return p.Akasse_AkasseMedlemsskab.Where (m =>(((m.Akasse_Medlem.Akasse_Person.Cprnr == CprNr) &&  (m.Akasse_Medlem.Akasse_Akasse.AkasseNr == AkasseNr)) && (m.Udmeldelsesdato != null)));
	}
}
