$base = "C:\git\AppMAUICopilot\.github\skills\criar-servico-maui-workspace\iteration-1"
$e1w = "$base\eval-product-generic\with_skill\outputs"
$e1b = "$base\eval-product-generic\without_skill\outputs"
$e2w = "$base\eval-category-custom\with_skill\outputs"
$e2b = "$base\eval-category-custom\without_skill\outputs"
$e3w = "$base\eval-employee-minimal\with_skill\outputs"
$e3b = "$base\eval-employee-minimal\without_skill\outputs"

function C($f,$p){
    if(Test-Path $f){
        $content = Get-Content $f -Raw
        return [bool]($content -match $p)
    }
    return $false
}

$checks = @(
    [pscustomobject]@{eval="E1";assert="Model : BaseEntity";w=(C "$e1w\Model_Product.cs" ': BaseEntity');b=(C "$e1b\Model_Product.cs" ': BaseEntity')},
    [pscustomobject]@{eval="E1";assert="Service : BaseService<T>";w=(C "$e1w\ProductService.cs" ': BaseService<');b=(C "$e1b\ProductService.cs" ': BaseService<')},
    [pscustomobject]@{eval="E1";assert="Interface : IBaseService<T>";w=(C "$e1w\IProductService.cs" ': IBaseService<');b=(C "$e1b\IProductService.cs" ': IBaseService<')},
    [pscustomobject]@{eval="E1";assert="Endpoint const Products";w=(C "$e1w\ApiConfig_addition.txt" 'Products');b=(C "$e1b\ApiConfig_addition.txt" 'Products')},
    [pscustomobject]@{eval="E1";assert="DI registration";w=(C "$e1w\MauiProgram_addition.txt" 'IProductService');b=(C "$e1b\MauiProgram_addition.txt" 'IProductService')},
    [pscustomobject]@{eval="E1";assert="No hardcoded URL";w=(-not (C "$e1w\ProductService.cs" 'http://'));b=(-not (C "$e1b\ProductService.cs" 'http://'))},
    [pscustomobject]@{eval="E1";assert="XML docs";w=(C "$e1w\ProductService.cs" 'summary>');b=(C "$e1b\ProductService.cs" 'summary>')},
    [pscustomobject]@{eval="E1";assert="File-scoped namespace";w=(C "$e1w\ProductService.cs" 'Services;');b=(C "$e1b\ProductService.cs" 'Services;')},
    [pscustomobject]@{eval="E2";assert="Model : BaseEntity";w=(C "$e2w\Model_Category.cs" ': BaseEntity');b=(C "$e2b\Model_Category.cs" ': BaseEntity')},
    [pscustomobject]@{eval="E2";assert="Status filter method";w=(C "$e2w\ICategoryService.cs" 'Status');b=(C "$e2b\ICategoryService.cs" 'Status')},
    [pscustomobject]@{eval="E2";assert="Custom pagination logic";w=(C "$e2w\CategoryService.cs" 'CategoryPaged|private.*class');b=(C "$e2b\CategoryService.cs" 'CategoryPaged|private.*class')},
    [pscustomobject]@{eval="E2";assert="ApiException thrown";w=(C "$e2w\CategoryService.cs" 'throw new ApiException');b=(C "$e2b\CategoryService.cs" 'throw new ApiException')},
    [pscustomobject]@{eval="E2";assert="Endpoint const Categories";w=(C "$e2w\ApiConfig_addition.txt" 'Categories');b=(C "$e2b\ApiConfig_addition.txt" 'Categories')},
    [pscustomobject]@{eval="E2";assert="DI registration";w=(C "$e2w\MauiProgram_addition.txt" 'ICategoryService');b=(C "$e2b\MauiProgram_addition.txt" 'ICategoryService')},
    [pscustomobject]@{eval="E2";assert="XML docs";w=(C "$e2w\ICategoryService.cs" 'summary>');b=(C "$e2b\ICategoryService.cs" 'summary>')},
    [pscustomobject]@{eval="E3";assert="Model : BaseEntity";w=(C "$e3w\Model_Employee.cs" ': BaseEntity');b=(C "$e3b\Model_Employee.cs" ': BaseEntity')},
    [pscustomobject]@{eval="E3";assert="Service : BaseService<T>";w=(C "$e3w\EmployeeService.cs" ': BaseService<');b=(C "$e3b\EmployeeService.cs" ': BaseService<')},
    [pscustomobject]@{eval="E3";assert="Assumptions documented";w=(C "$e3w\summary.txt" 'Assumption');b=(C "$e3b\summary.txt" 'Assumption')},
    [pscustomobject]@{eval="E3";assert="Endpoint const Employees";w=(C "$e3w\ApiConfig_addition.txt" 'Employees');b=(C "$e3b\ApiConfig_addition.txt" 'Employees')},
    [pscustomobject]@{eval="E3";assert="DI registration";w=(C "$e3w\MauiProgram_addition.txt" 'IEmployeeService');b=(C "$e3b\MauiProgram_addition.txt" 'IEmployeeService')},
    [pscustomobject]@{eval="E3";assert="No hardcoded URL";w=(-not (C "$e3w\EmployeeService.cs" 'http://'));b=(-not (C "$e3b\EmployeeService.cs" 'http://'))}
)

$wPass=0; $bPass=0
foreach ($c in $checks) {
    $ws2 = if ($c.w) { "PASS" } else { "FAIL" }
    $bs2 = if ($c.b) { "PASS" } else { "FAIL" }
    if ($c.w) { $wPass++ }
    if ($c.b) { $bPass++ }
    Write-Output "$($c.eval) | $($c.assert.PadRight(30)) | with=$ws2 | base=$bs2"
}
Write-Output ""
Write-Output "TOTAL: with_skill=$wPass/$($checks.Count)  baseline=$bPass/$($checks.Count)"
