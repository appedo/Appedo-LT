<?xml version="1.0" encoding="ISO-8859-1"?>
<!-- Edited by XMLSpy® -->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:template match="//report">
    <html>
      <body>
        <xsl:apply-templates select="error"/>
        <br/>
      </body>
    </html>
  </xsl:template>

  <xsl:template match="error">
    <h4> Logs</h4>

    <table border="1">
      <tr bgcolor="#9acd32">
        <th>loadgen</th>
        <th>epochtimestamp</th>
        <th>address</th>
        <th>responsecode</th>
        <th>httpresponsemessage</th>
        <th>threadgroupname</th>
        <th>responsesize</th>
      </tr>
      <xsl:for-each select="val">
        <tr>
          <td>
            <xsl:value-of select="@loadgen"/>
          </td>
          <td>
            <xsl:value-of select="@epochtimestamp"/>
          </td>
		  <td>
            <xsl:value-of select="@address"/>
          </td>
          <td>
            <xsl:value-of select="@responsecode"/>
          </td>
          <td>
            <xsl:value-of select="@httpresponsemessage"/>
          </td>
          <td>
            <xsl:value-of select="@threadgroupname"/>
          </td>
          <td>
            <xsl:value-of select="@responsesize"/>
          </td>
        </tr>
      </xsl:for-each>
    </table>

  </xsl:template>

</xsl:stylesheet>

