<ResourceDictionary 
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:sys="clr-namespace:System;assembly=mscorlib">

    <!-- TODO -->
    <!-- Add configuration settings specific for your app -->
    
    <!-- ChannelName can be more or less anything, perhaps simply your app name. -->
    <sys:String x:Key="ChannelName">NameOfYourChannel</sys:String>
    
    <!-- This is the Uri to the registration service. When developing, this -->
    <!-- is probably something like http://localhost:1234/ModuleName/Registration -->
    <!-- For production, you'll need to change this to point at your production server -->
    <sys:String x:Key="RegistrationServiceUri">http://[hostname].com/ModuleName/Registration</sys:String>
    <!--<sys:String x:Key="RegistrationServiceUri">http://localhost:1234/Sample/Registration</sys:String>-->

    <!-- This is an optional list of Uri:s where your live tiles can be hosted -->
    <!-- Again, you'll need to change this to point at your own host -->
    <sys:String x:Key="AllowedTileUris">http://[hostname].com</sys:String>
    <!--<sys:String x:Key="AllowedTileUris">http://localhost:1234</sys:String>-->

    <!-- Set this to true if you want to enable Live Tiles -->
    <sys:String x:Key="RequestLiveTiles">true</sys:String>

    <!-- Set this to true if you want to enable Toasts -->
    <sys:String x:Key="RequestToasts">true</sys:String>

</ResourceDictionary>
