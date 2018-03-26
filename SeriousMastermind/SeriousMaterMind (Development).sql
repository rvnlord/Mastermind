select * from tblStatistics
select * from tblstories;

CREATE TABLE tblStatistics
(
  Id INT PRIMARY KEY,
  Name VARCHAR(100),
  CodeLength INT,
  Tries INT
);

drop table tblstories;

CREATE TABLE tblStories
(
  Id INT PRIMARY KEY,
  Content TEXT
);

ALTER TABLE tblStory RENAME TO tblStories;

INSERT INTO tblStories VALUES (0, 'The Battle of Culloden was the final confrontation of the Jacobite rising of 1745 and part of a religious civil war in Britain. On 16 April 1746, the Jacobite forces of Charles Edward Stuart were decisively defeated by loyalist troops commanded by William Augustus, Duke of Cumberland, near Inverness in the Scottish Highlands.\nQueen Anne, the last monarch of the House of Stuart, died in 1714, with no living children. Under the terms of the Act of Settlement 1701, she was succeeded by her second cousin George I of the House of Hanover, who was a descendant of the Stuarts through his maternal grandmother, Elizabeth, a daughter of James VI and I. The Hanoverian victory at Culloden halted the Jacobite intent to overthrow the House of Hanover and restore the House of Stuart to the British throne; Charles Stuart never again tried to challenge Hanoverian power in Great Britain. The conflict was the last pitched battle fought on British soil.\nCharles Stuart''s Jacobite army consisted largely of Catholics and Scottish Episcopalians – mainly Scots but with a small detachment of Englishmen from the Manchester Regiment. The Jacobites were supported and supplied by the Kingdom of France from Irish and Scots units in French service. A composite battalion of infantry ("Irish Picquets"), comprising detachments from each of the regiments of the Irish Brigade plus one squadron of Irish in the French army, served at the battle alongside the regiment of Royal Scots (Royal Écossais) raised the previous year to support the Stuart claim. The British Government (Hanoverian loyalist) forces were mostly Protestants – English, along with a significant number of Scottish Lowlanders and Highlanders, a battalion of Ulstermen, and some Hessians from Germany, and Austrians. The quick and bloody battle on Culloden Moor was over in less than an hour, when after an unsuccessful Highland charge against the government lines, the Jacobites were routed and driven from the field.\nBetween 1,500 and 2,000 Jacobites were killed or wounded in the brief battle. In contrast, only about 300 government soldiers were killed or wounded.\nThe battle and its aftermath continue to arouse strong feelings: the University of Glasgow awarded the Duke of Cumberland an honorary doctorate, but many modern commentators allege that the aftermath of the battle and subsequent crackdown on Jacobitism were brutal, and earned Cumberland the sobriquet "Butcher". Efforts were subsequently made to further integrate the comparatively wild Scottish Highlands into the Kingdom of Great Britain; civil penalties were introduced to weaken Gaelic culture and undermine the Scottish clan system.');

INSERT INTO tblStatistics VALUES (13, 'Test', 6, 1);
