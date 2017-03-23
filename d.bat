rd /s /q "D:\Game\Steam\steamapps\common\RimWorld\Mods\Translation Framework\About"
rd /s /q "D:\Game\Steam\steamapps\common\RimWorld\Mods\Translation Framework\Assemblies"
rd /s /q "D:\Game\Steam\steamapps\common\RimWorld\Mods\Translation Framework\Defs"
rd /s /q "D:\Game\Steam\steamapps\common\RimWorld\Mods\Translation Framework\Languages"
xcopy About "D:\Game\Steam\steamapps\common\RimWorld\Mods\Translation Framework\About" /e /d /i /y
xcopy Assemblies "D:\Game\Steam\steamapps\common\RimWorld\Mods\Translation Framework\Assemblies" /e /d /i /y
xcopy Defs "D:\Game\Steam\steamapps\common\RimWorld\Mods\Translation Framework\Defs" /e /d /i /y
xcopy Languages "D:\Game\Steam\steamapps\common\RimWorld\Mods\Translation Framework\Languages" /e /d /i /y
copy LICENSE "D:\Game\Steam\steamapps\common\RimWorld\Mods\Translation Framework\LICENSE" /y