<?xml version="1.0" encoding="ISO-8859-1"?>
<!-- Edited by XMLSpy® -->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:template match="//report">
    <html>
      <head>
        <title>Appedo</title>
        <link rel="shortcut icon" type="image/x-icon" href="images/appedo-16.ico" />
        <link rel="stylesheet" href="css/morris.css" />
        <script src="js/raphael-min.js"></script>
        <script src="js/jquery-1.8.2.min.js"></script>
        <script src="js/morris-0.4.1.min.js"></script>
        <script>
          function drawCharts()
          {
            drawSummaryGraph();
            drawErrorGraph();
            drawPageResponseGraph();
            drawVUserGraph();
          }        
        </script>
        <style>
          body {
          font-family: "Helvetica Neue", Helvetica, Arial, sans-serif;
          font-size: 14px;
          line-height: 1.42857143;
          color: #333;
          background-color: #fff;
          }

          .graphheadingrow
          {
          text-align: center;
          border-bottom: 1px solid #1b639e;
          }

          .graph
          {
          width: 400px; 
          height: 250px;
          padding: 25px;
          }
        </style>
      </head>
      <body onload="drawCharts();">
        <xsl:apply-templates select="summaryreport"/>
        <br/>
        <xsl:for-each select="script">
          <span>
            Script Name: <b>
              <xsl:value-of select="@name"/>
            </b>
          </span>
          <br/>
          <xsl:apply-templates select="settings"/>
          <xsl:apply-templates select="graphs"/>
          <xsl:apply-templates select="requestresponse"/>
          <xsl:apply-templates select="pageresponsegraph"/>
          <xsl:apply-templates select="errorgraph"/>
          <xsl:apply-templates select="containerresponse"/>
          <xsl:apply-templates select="transactions"/>
          <xsl:apply-templates select="errorcount"/>
          <xsl:apply-templates select="errorcode"/>
          <xsl:apply-templates select="vuserrungraph"/>
        </xsl:for-each>
      </body>
    </html>
  </xsl:template>

  <xsl:template match="summaryreport">
    <h1 style="width:600px; padding-bottom: 6px; background-color:#31c0be; color:#FFFFFF; text-align: center;">Appedo</h1>

    <span>
      <b>Report Name:  </b>
      <xsl:value-of select="//report/@reportname"/>
    </span>

    <h4> Summary Report</h4>
    <table border="1" cellspacing="0">
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

  <xsl:template match="settings">
    <h4> Settings</h4>
    <table border="1" cellspacing="0" cellpadding="3">
      <xsl:for-each select="val">
        <tr>
          <td  style="width: 160px;">
            <b>Browser Cache</b>
          </td>
          <td style="width: 100px;">
            <xsl:value-of select="@browserCache"/>
          </td>
          <td  style="width: 160px;">
            <b>Replay Think Time</b>
          </td>
          <td style="width: 100px;">
            <xsl:value-of select="@replayThinkTime"/>
          </td>
        </tr>
        <tr>
          <td>
            <b>Bandwidth</b>
          </td>
          <td>
            <xsl:value-of select="@bandwidth"/>
          </td>
          <td >
            <b>Parallel Connections</b>
          </td>
          <td style="text-align:right">
            <xsl:value-of select="@parallelconnections"/>
          </td>
        </tr>
        <tr>
          <td>
            <b>Start User Count</b>
          </td>
          <td style="text-align:right">
            <xsl:value-of select="@startUserId"/>
          </td>
          <td>
            <b>Max Users</b>
          </td>
          <td style="text-align:right">
            <xsl:value-of select="@maxUser"/>
          </td>
        </tr>
        <tr>
          <td>
            <b>Mode</b>
          </td>
          <td>
            <xsl:value-of select="@type"/>
          </td>
          <xsl:if test="@type='Duration'">
            <td>
              <b>Duration</b>
            </td>
            <td style="text-align:right;">
              <xsl:value-of select="@durationTime"/>
            </td>
          </xsl:if>
          <xsl:if test="@type='Iteration'">
            <td>
              <b>Iterations</b>
            </td>
            <td style="text-align:right">
              <xsl:value-of select="@iterations"/>
            </td>
          </xsl:if>
        </tr>
        <tr>
          <td>
            <b>Increment User</b>
          </td>
          <td style="text-align:right">
            <xsl:value-of select="@incrementUser"/>
          </td>
          <td>
            <b>Increment Time</b>
          </td>
          <td style="text-align:right">
            <xsl:value-of select="@incrementTime"/>
          </td>
        </tr>
      </xsl:for-each>
    </table>
  </xsl:template>

  <xsl:template match="graphs">
    <table style="width: 875px;" cellpadding="10" cellspacing="0">
      <tr>
        <td>
          <div id="{concat('vuser_', ../@id)}" class="graph"></div>
        </td>
        <td style="width: 15px;"></td>
        <td>
          <div id="{concat('hps_', ../@id)}" class="graph"></div>
        </td>
      </tr>
      <tr>
        <td class="graphheadingrow">
          <b>Virtual User Running Graph</b>
        </td>
        <td></td>
        <td class="graphheadingrow">
          <b>Hits per Sec Graph</b>
        </td>
      </tr>
      <tr>
        <td>
          <div id="{concat('kbps_', ../@id)}" style="width: 400px; height: 250px;"></div>
        </td>
        <td></td>
        <td>
          <div id="{concat('pageres_', ../@id)}" style="width: 400px; height: 250px;"></div>
        </td>
      </tr>
      <tr>
        <td class="graphheadingrow">
          <b>Throughput per Sec Graph (KBps)</b>
        </td>
        <td></td>
        <td class="graphheadingrow">
          <b>Page Response Time Graph</b>
        </td>
      </tr>
    </table>
    <script>
      function drawSummaryGraph() {      
      new Morris.Line({
      element: 'hps_<xsl:value-of select="../@id"/>',
      data: [
      <xsl:for-each select="val">
        <xsl:choose>
          <xsl:when test="position() != last()">
            { time: '<xsl:value-of select="@localtime"/>', hitspersec: <xsl:value-of select="@hitpersec"/> },
          </xsl:when>
          <xsl:otherwise>
            { time: '<xsl:value-of select="@localtime"/>', hitspersec: <xsl:value-of select="@hitpersec"/> }
          </xsl:otherwise>
        </xsl:choose>
      </xsl:for-each>
      ],
      xkey: 'time',
      ykeys: ['hitspersec'],
      labels: ['Hits per Sec'],
      lineColors: ['#D26B16']
      });

      new Morris.Line({
      element: 'kbps_<xsl:value-of select="../@id"/>',
      data: [
      <xsl:for-each select="val">
        <xsl:choose>
          <xsl:when test="position() != last()">
            { time: '<xsl:value-of select="@localtime"/>', kbps: <xsl:value-of select="@kbps"/> },
          </xsl:when>
          <xsl:otherwise>
            { time: '<xsl:value-of select="@localtime"/>', kbps: <xsl:value-of select="@kbps"/> }
          </xsl:otherwise>
        </xsl:choose>
      </xsl:for-each>
      ],
      xkey: 'time',
      ykeys: ['kbps'],
      labels: ['KBps'],
      lineColors: ['#800000']
      });

      new Morris.Line({
      element: 'art_<xsl:value-of select="../@id"/>',
      data: [
      <xsl:for-each select="val">
        <xsl:choose>
          <xsl:when test="position() != last()">
            { time: '<xsl:value-of select="@localtime"/>', avgresponsetime: <xsl:value-of select="@avgresponsetime"/> },
          </xsl:when>
          <xsl:otherwise>
            { time: '<xsl:value-of select="@localtime"/>', avgresponsetime: <xsl:value-of select="@avgresponsetime"/> }
          </xsl:otherwise>
        </xsl:choose>
      </xsl:for-each>
      ],
      xkey: 'time',
      ykeys: ['avgresponsetime'],
      labels: ['Avg. Response Time'],
      lineColors: ['#FFA500']
      });
      }
    </script>
  </xsl:template>

  <xsl:template match="pageresponsegraph">
    <script>
      function drawPageResponseGraph() {
        new Morris.Line({
        element: 'pageres_<xsl:value-of select="../@id"/>',
        data: [
        <xsl:for-each select="val">
          <xsl:choose>
            <xsl:when test="position() != last()">
              { time: '<xsl:value-of select="@localtime"/>', resptime: <xsl:value-of select="@resptime"/> },
            </xsl:when>
            <xsl:otherwise>
              { time: '<xsl:value-of select="@localtime"/>', resptime: <xsl:value-of select="@resptime"/> }
            </xsl:otherwise>
          </xsl:choose>
        </xsl:for-each>
        ],
        xkey: 'time',
        ykeys: ['resptime'],
        labels: ['Avg. Response Time'],
        lineColors: ['#2616D2']
        });
      }
    </script>
  </xsl:template>

  <xsl:template match="requestresponse">
    <h4> Request response</h4>
    <table border="1" cellspacing="0">
      <tr bgcolor="#9acd32">
        <th>Containername</th>
        <th>Address</th>
        <th>Hit Count</th>
        <th>Throughput(Byte)</th>
        <th>Min(ms)</th>
        <th>Max(ms)</th>
        <th>Avg(ms)</th>
        <th>Min TTFB(ms)</th>
        <th>Max TTFB(ms)</th>
        <th>Avg TTFB(ms)</th>
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
            <xsl:value-of select="@hitcount"/>
          </td>
          <td style="text-align:right">
            <xsl:value-of select="@throughput"/>
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
          <td style="text-align:right">
            <xsl:value-of select="format-number(@minttfb,'#.000')"/>
          </td>
          <td style="text-align:right">
            <xsl:value-of select="format-number(@maxttfb,'#.000')"/>
          </td>
          <td style="text-align:right">
            <xsl:value-of select="format-number(@avgttfb,'#.000')"/>
          </td>
        </tr>
      </xsl:for-each>
    </table>

  </xsl:template>

  <xsl:template match="containerresponse">
    <h4> Container response</h4>

    <table border="1" cellspacing="0">
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

    <table border="1" cellspacing="0">
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

  <xsl:template match="errorcount">
    <h4>Error count</h4>

    <table border="1" cellspacing="0">
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

    <table border="1" cellspacing="0">
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
  
  <xsl:template match="errorgraph">
    <table style="width: 875px;" cellpadding="10" cellspacing="0">
      <tr>
         <td>
          <div id="{concat('art_', ../@id)}" class="graph"></div>
        </td>
        <td style="width: 15px;"></td>
        <td>
          <div id="{concat('error_', ../@id)}" class="graph"></div>
        </td>
      </tr>
      <tr>
        <td class="graphheadingrow">
          <b>Request Response Graph</b>
        </td>
        <td></td>
        <td class="graphheadingrow">
          <b>Error Graph</b>
        </td>
      </tr>
    </table>
    <script>
      function drawErrorGraph() {
        new Morris.Line({
            element: 'error_<xsl:value-of select="../@id"/>',
            data: [
            <xsl:for-each select="val">
              <xsl:choose>
                <xsl:when test="position() != last()">
                  { time: '<xsl:value-of select="@localtime"/>', errorcount: <xsl:value-of select="@errorcount"/> },
                </xsl:when>
                <xsl:otherwise>
                  { time: '<xsl:value-of select="@localtime"/>', errorcount: <xsl:value-of select="@errorcount"/> }
                </xsl:otherwise>
              </xsl:choose>
            </xsl:for-each>
            ],
            xkey: 'time',
            ykeys: ['errorcount'],
            labels: ['Error Count'],
            lineColors: ['#D21629']
        });
      }
    </script>
  </xsl:template>

  <xsl:template match="vuserrungraph">
    <script>
      function drawVUserGraph() {
        new Morris.Line({
        element: 'vuser_<xsl:value-of select="../@id"/>',
        data: [
        <xsl:for-each select="val">
          <xsl:choose>
            <xsl:when test="position() != last()">
              { time: '<xsl:value-of select="@localtime"/>', vuserrunning: <xsl:value-of select="@vuserrunning"/> },
            </xsl:when>
            <xsl:otherwise>
              { time: '<xsl:value-of select="@localtime"/>', vuserrunning: <xsl:value-of select="@vuserrunning"/> }
            </xsl:otherwise>
          </xsl:choose>
        </xsl:for-each>
        ],
        xkey: 'time',
        ykeys: ['vuserrunning'],
        labels: ['VUsers Running'],
        lineColors: ['#68D216']
        });
      }
    </script>
  </xsl:template>

</xsl:stylesheet>

