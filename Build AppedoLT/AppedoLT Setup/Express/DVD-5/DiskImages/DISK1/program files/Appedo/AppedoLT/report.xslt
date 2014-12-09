<?xml version="1.0" encoding="ISO-8859-1"?>
<!-- Edited by XMLSpy® -->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:template match="//report">
    <html>
      <body>
        <xsl:apply-templates select="summaryreport"/>
        <br/>
        <xsl:for-each select="script">
          <span>
            Script Name: <b>
              <xsl:value-of select="@name"/>
            </b>
          </span>
          <br/>
          <xsl:apply-templates select="requestresponse"/>
          <xsl:apply-templates select="containerresponse"/>
          <xsl:apply-templates select="transactions"/>
          <xsl:apply-templates select="throughput"/>
          <xsl:apply-templates select="hitcount"/>
          <xsl:apply-templates select="errorcount"/>
          <xsl:apply-templates select="errorcode"/>
        </xsl:for-each>
      </body>
    </html>
  </xsl:template>

  <xsl:template match="summaryreport">
    <h4> Summary Report</h4>
    <table border="1">
      <xsl:for-each select="val">
        <tr>
          <td style="bgcolor=#9acd32">
            <b>Start Time</b>
          </td>
          <td style="text-align:right">
            <xsl:value-of select="@starttime"/>
          </td>
        </tr>
        <tr>
          <td style="text-align:left; bgcolor=#9acd32">
            <b>End Time</b>
          </td>
          <td style="text-align:right">
            <xsl:value-of select="@endtime"/>
          </td>
        </tr>
        <tr>
          <td style="text-align:left; bgcolor=#9acd32">
            <b>Duration(sec)</b>
          </td>
          <td style="text-align:right">
            <xsl:value-of select="@durationsec"/>
          </td>
        </tr>
        <tr>
          <td style="text-align:left; bgcolor=#9acd32">
            <b>User Count</b>
          </td>
          <td style="text-align:right">
            <xsl:value-of select="@usercount"/>
          </td>
        </tr>
        <tr>
          <td style="text-align:left; bgcolor=#9acd32">
            <b>Total Hits</b>
          </td>
          <td style="text-align:right">
            <xsl:value-of select="@totalhits"/>
          </td>
        </tr>
        <tr>
          <td style="text-align:left; bgcolor=#9acd32">
            <b>Average Request Response(sec)</b>
          </td>
          <td style="text-align:right">
            <xsl:value-of select="format-number(@avgresponse,'#.000')"/>
          </td>
        </tr>
        <tr>
          <td style="text-align:left; bgcolor=#9acd32">
            <b>Average Page Response(sec)</b>
          </td>
          <td style="text-align:right">
            <xsl:value-of select="format-number(@avgpageresponse,'#.000')"/>
          </td>
        </tr>
        <tr>
          <td style="text-align:left; bgcolor=#9acd32">
            <b>Average Hits/Sec</b>
          </td>
          <td style="text-align:right">
            <xsl:value-of select="format-number(@avghits,'#.000')"/>
          </td>
        </tr>
        <tr>
          <td style="text-align:left; bgcolor=#9acd32">
            <b> Total Throughput(MB)</b>
          </td>
          <td style="text-align:right">
            <xsl:value-of select="format-number(@totalthroughput,'#.000')"/>
          </td>
        </tr>
        <tr>
          <td style="text-align:left; bgcolor=#9acd32">
            <b>Average Throughput(Mbps)</b>
          </td>
          <td style="text-align:right">
            <xsl:value-of select="format-number(@avgthroughput,'#.000')"/>
          </td>
        </tr>
        <tr>
          <td style="text-align:left; bgcolor=#9acd32">
            <b>Total Errors</b>
          </td>
          <td style="text-align:right">
            <xsl:value-of select="@totalerrors"/>
          </td>
        </tr>
        <tr>
          <td style="text-align:left; bgcolor=#9acd32">
            <b>Total pages</b>
          </td>
          <td style="text-align:right">
            <xsl:value-of select="@totalpage"/>
          </td>
        </tr>
        <tr>
          <td style="text-align:left; bgcolor=#9acd32">
            <b>Response Code 200 Count</b>
          </td>
          <td style="text-align:right">
            <xsl:value-of select="@reponse200"/>
          </td>
        </tr>
        <tr>
          <td style="text-align:left; bgcolor=#9acd32">
            <b>Response Code 300 Count</b>
          </td>
          <td style="text-align:right">
            <xsl:value-of select="@reponse300"/>
          </td>
        </tr>
        <tr>
          <td style="text-align:left; bgcolor=#9acd32">
            <b>Response Code 400 Count</b>
          </td>
          <td style="text-align:right">
            <xsl:value-of select="@reponse400"/>
          </td>
        </tr>
        <tr>
          <td style="text-align:left; bgcolor=#9acd32">
            <b>Response Code 500 Count</b>
          </td>
          <td style="text-align:right">
            <xsl:value-of select="@reponse500"/>
          </td>
        </tr>
      </xsl:for-each>
    </table>

  </xsl:template>

  <xsl:template match="requestresponse">
    <h4> Request response</h4>

    <table border="1">
      <tr bgcolor="#9acd32">
        <th>Containername</th>
        <th>Address</th>
        <th>Min(ms)</th>
        <th>Max(ms)</th>
        <th>Avg(ms)</th>
      </tr>
      <xsl:for-each select="val">
        <tr>
          <td>
            <xsl:value-of select="@containername"/>
          </td>
          <td>
            <xsl:value-of select="@address"/>
          </td>
          <td style="text-align:right">
            <xsl:value-of select="format-number(@min,'#.000')"/>
          </td>
          <td style="text-align:right">
            <xsl:value-of select="format-number(@max,'#.000')"/>
          </td>
          <td style="text-align:right">
            <xsl:value-of select="format-number(@avg,'#.000')"/>
          </td>
        </tr>
      </xsl:for-each>
    </table>

  </xsl:template>

  <xsl:template match="containerresponse">
    <h4> Container response</h4>

    <table border="1">
      <tr bgcolor="#9acd32">
        <th>Containername</th>
        <th>Min(ms)</th>
        <th>Max(ms)</th>
        <th>Avg(ms)</th>
      </tr>
      <xsl:for-each select="val">
        <tr>
          <td>
            <xsl:value-of select="@containername"/>
          </td>
         <td style="text-align:right">
            <xsl:value-of select="format-number(@min,'#.000')"/>
          </td>
          <td style="text-align:right">
            <xsl:value-of select="format-number(@max,'#.000')"/>
          </td>
          <td style="text-align:right">
            <xsl:value-of select="format-number(@avg,'#.000')"/>
          </td>
        </tr>
      </xsl:for-each>
    </table>

  </xsl:template>

  <xsl:template match="transactions">
    <h4> Transactions response</h4>

    <table border="1">
      <tr bgcolor="#9acd32">
        <th>Transaction Name</th>
        <th>Min(ms)</th>
        <th>Max(ms)</th>
        <th>Avg(ms)</th>
      </tr>
      <xsl:for-each select="val">
        <tr>
          <td>
            <xsl:value-of select="@transactionname"/>
          </td>
          <td style="text-align:right">
            <xsl:value-of select="format-number(@min,'#.000')"/>
          </td>
          <td style="text-align:right">
            <xsl:value-of select="format-number(@max,'#.000')"/>
          </td>
          <td style="text-align:right">
            <xsl:value-of select="format-number(@avg,'#.000')"/>
          </td>
        </tr>
      </xsl:for-each>
    </table>

  </xsl:template>

  <xsl:template match="throughput">
    <h4>Throughput</h4>

    <table border="1">
      <tr bgcolor="#9acd32">
        <th>Containername</th>
        <th>Address</th>
        <th>Total(bytes)</th>
      </tr>
      <xsl:for-each select="val">
        <tr>
          <td>
            <xsl:value-of select="@containername"/>
          </td>
          <td>
            <xsl:value-of select="@address"/>
          </td>
          <td style="text-align:right">
            <xsl:value-of select="@total"/>
          </td>
        </tr>
      </xsl:for-each>
    </table>

  </xsl:template>

  <xsl:template match="hitcount">
    <h4>Hit count</h4>

    <table border="1">
      <tr bgcolor="#9acd32">
        <th>Containername</th>
        <th>Address</th>
        <th>Count</th>
      </tr>
      <xsl:for-each select="val">
        <tr>
          <td>
            <xsl:value-of select="@containername"/>
          </td>
          <td>
            <xsl:value-of select="@address"/>
          </td>
          <td style="text-align:right">
            <xsl:value-of select="@count"/>
          </td>
        </tr>
      </xsl:for-each>
    </table>

  </xsl:template>

  <xsl:template match="errorcount">
    <h4>Error count</h4>

    <table border="1">
      <tr bgcolor="#9acd32">
        <th>Containername</th>
        <th>Address</th>
        <th>Count</th>
      </tr>
      <xsl:for-each select="val">
        <tr>
          <td>
            <xsl:value-of select="@containername"/>
          </td>
          <td>
            <xsl:value-of select="@address"/>
          </td>
          <td style="text-align:right">
            <xsl:value-of select="@count"/>
          </td>
        </tr>
      </xsl:for-each>
    </table>
  </xsl:template>

  <xsl:template match="errorcode">
    <h4>Error Description</h4>

    <table border="1">
      <tr bgcolor="#9acd32">
        <th>Errorcode</th>
        <th>Message</th>
        <th>Count</th>
      </tr>
      <xsl:for-each select="val">
        <tr>
          <td>
            <xsl:value-of select="@errorcode"/>
          </td>
          <td>
            <xsl:value-of select="@message"/>
          </td>
          <td style="text-align:right">
            <xsl:value-of select="@count"/>
          </td>
        </tr>
      </xsl:for-each>
    </table>
  </xsl:template>

</xsl:stylesheet>

